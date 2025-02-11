using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounterUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _isAnimated = false;
    [SerializeField] private List<Image> _countImages = new List<Image>();

    public void SetMaxCount(int count)
    {
        if (count > _countImages.Count)
        {
            Debug.LogWarning("Count is out of range.");
            return;
        }

        for (int i = 0; i < _countImages.Count; i++)
        {
            if (i < count)
            {
                _countImages[i].gameObject.SetActive(true);
                if(_isAnimated)
                {
                    Animator anim = _countImages[i].GetComponent<Animator>();
                    anim.enabled = false;
                }
            }
            else
            {
                _countImages[i].gameObject.SetActive(false);
            }
        }
    }
    public void SetCount(int count)
    {
        if(count > _countImages.Count)
        {
            Debug.LogWarning("Count is out of range.");
            return;
        }

        for (int i = 0; i < _countImages.Count; i++)
        {
            if (i < count)
            {
                _countImages[i].gameObject.SetActive(true);
                if (_isAnimated)
                {
                    Animator heart = _countImages[i].GetComponent<Animator>();
                    heart.enabled = false;
                }
            }
            else
            {
                if (_isAnimated)
                {
                    Animator heart = _countImages[i].GetComponent<Animator>();
                    heart.enabled = true;
                    StartCoroutine(DelayDisable(_countImages[i].gameObject, 1f));
                }
                else
                {
                    _countImages[i].gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator DelayDisable(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);
    }
}
