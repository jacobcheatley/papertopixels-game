using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class Thumbs : MonoBehaviour
{
    [SerializeField] private GameObject thumbPrefab;

    private float spacing = 141 + 20;
    private List<GameObject> thumbs = new List<GameObject>();
    private int index = 0;
    private RectTransform rectTransform;

    // TODO: Proper state handling
    private GamePadState state;
    private GamePadState prevState;

    void Start()
    {
        prevState = state = GamePad.GetState(PlayerIndex.One);
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(LoadAllThumbs());
    }

    // TODO: Caching would be really really nice
    private IEnumerator LoadAllThumbs()
    {
        for (int i = 0; i < 2; i++)
        {
            Texture2D tex;

            if ((tex = FileCache.LoadThumb(i)) == null)
            {
                string url = $"http://papertopixels.tk/thumb/{i}";
                WWW www = new WWW(url);
                yield return www;
                tex = www.texture;
                FileCache.SaveThumb(tex, i);
            }

            GameObject image = Instantiate(thumbPrefab, transform);
            image.name = $"Thumb {i}";
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            image.GetComponent<Image>().sprite = sprite;
            AddThumb(image);
        }
    }

    void Update()
    {
        if (thumbs.Count > 0)
        {
            // TODO: Actual first player
            prevState = state;
            state = GamePad.GetState(PlayerIndex.One);

            if (Controller.LeftPress(prevState, state))
            {
                index--;
                if (index < 0) index = 0;
                else SnapToIndex();
            }
            else if (Controller.RightPress(prevState, state))
            {
                index++;
                if (index >= thumbs.Count) index = thumbs.Count - 1;
                else SnapToIndex();
            }
        }
    }

    private void SnapToIndex()
    {
        // TODO: Nice animation on switch
        float target = index * spacing;
        rectTransform.localPosition = new Vector3(-target, rectTransform.localPosition.y);
    }

    private void AddThumb(GameObject thumb)
    {
        thumb.transform.localPosition = Vector3.right * thumbs.Count * spacing;
        thumbs.Add(thumb);
    }
}
