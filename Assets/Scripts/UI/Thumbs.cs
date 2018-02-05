using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using XInputDotNetPure;

public class Thumbs : MonoBehaviour
{
    [SerializeField] private GameObject thumbPrefab;
    [SerializeField] private GameObject preview;
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private float spacing = 141 + 30;
    private List<GameObject> thumbs = new List<GameObject>();
    private List<int> ids = new List<int>();
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
        preview.SetActive(true);
    }

    private IEnumerator LoadAllThumbs()
    {
        int[] maps = {0};
        using (UnityWebRequest www = UnityWebRequest.Get("http://papertopixels.tk/maps"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
                Debug.Log(www.error);
            else
            {
                MapList mapList = JsonUtility.FromJson<MapList>(www.downloadHandler.text);
                maps = mapList.maps;
            }
        }

        foreach (int i in maps)
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
            AddThumb(image, i);
        }
    }

    void Update()
    {
        if (thumbs.Count > 0)
        {
            prevState = state;
            try
            {
                state = GamePad.GetState(Persistent.LobbyController);
            }
            catch (Exception)
            {
                // Ignore - only happens on first load
            }

            if (Controller.LeftPress(prevState, state))
            {
                index--;
                StartCoroutine(BounceSize(leftArrow));
                if (index < 0) index = 0;
                else SnapToIndex();
            }
            else if (Controller.RightPress(prevState, state))
            {
                index++;
                StartCoroutine(BounceSize(rightArrow));
                if (index >= thumbs.Count) index = thumbs.Count - 1;
                else SnapToIndex();
            }
            else if (state.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released)
            {
                SceneControl.ToGame(ids[index]);
            }
        }
    }

    private IEnumerator BounceSize(Image image)
    {
        image.rectTransform.localScale = Vector3.one * 0.8f;
        yield return new WaitForSeconds(0.1f);
        image.rectTransform.localScale = Vector3.one * 1.1f;
        yield return new WaitForSeconds(0.1f);
        image.rectTransform.localScale = Vector3.one * 1f;
    }

    private void SnapToIndex()
    {
        float target = index * spacing;
        rectTransform.localPosition = new Vector3(-target, rectTransform.localPosition.y);

        leftArrow.color = new Color(1, 1, 1,  index == 0 ? 0 : 0.5f);
        rightArrow.color = new Color(1, 1, 1,  index == thumbs.Count - 1 ? 0 : 0.5f);
    }

    private void AddThumb(GameObject thumb, int id)
    {
        thumb.transform.localPosition = Vector3.right * thumbs.Count * spacing;
        thumbs.Add(thumb);
        ids.Add(id);
    }
}

// JSONUtility is dumb
class MapList
{
    public int[] maps;
}