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

    private IEnumerator LoadAllThumbs()
    {
        for (int i = 0; i < 2; i++)
        {
            string url = $"http://papertopixels.tk/thumb/{i}";
            WWW www = new WWW(url);
            yield return www;

            GameObject image = Instantiate(thumbPrefab, transform);
            image.name = $"Thumb {i}";
            Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
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
            bool left =
                (state.DPad.Left == ButtonState.Pressed && prevState.DPad.Left == ButtonState.Released) ||
                (state.ThumbSticks.Left.X < -0.3f && prevState.ThumbSticks.Left.X > -0.3f) ||
                (state.Buttons.LeftShoulder == ButtonState.Pressed && prevState.Buttons.LeftShoulder == ButtonState.Released);
            bool right =
                (state.DPad.Right == ButtonState.Pressed && prevState.DPad.Right == ButtonState.Released) ||
                (state.ThumbSticks.Left.X > 0.3f && prevState.ThumbSticks.Left.X < 0.3f) ||
                (state.Buttons.RightShoulder == ButtonState.Pressed && prevState.Buttons.RightShoulder == ButtonState.Released);

            if (left)
            {
                index--;
                if (index < 0) index = 0;
                else SnapToIndex();
            }
            else if (right)
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
        Debug.Log(target);
        rectTransform.localPosition = new Vector3(-target, rectTransform.localPosition.y);
    }

    private void AddThumb(GameObject thumb)
    {
        thumb.transform.localPosition = Vector3.right * thumbs.Count * spacing;
        thumbs.Add(thumb);
    }
}
