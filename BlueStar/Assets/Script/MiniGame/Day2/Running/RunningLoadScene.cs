using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class RunningLoadScene : MonoBehaviour
{
    private CanvasGroup blackBG;
    public string sceneToAdd;
    public string sceneToUnload;
    private PlayableDirector _director;
    
    void Start()
    {
        blackBG = GameObject.Find("------UI------/UI_2D/BlackBG").gameObject.GetComponent<CanvasGroup>();
        _director = this.GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(UpdateScene());
            this.GetComponent<Collider>().enabled = false;
            if (_director!=null)
            {
                _director.Play();
            }
        }
    }

    private IEnumerator UpdateScene()
    {
        blackBG.DOFade(1, 0.5f);
        yield return new WaitForSeconds(1f);

        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            yield return SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        if (!string.IsNullOrEmpty(sceneToAdd))
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToAdd, LoadSceneMode.Additive);
            yield return loadOp;

            Scene loadedScene = SceneManager.GetSceneByName(sceneToAdd);
            if (loadedScene.IsValid())
            {
                SceneManager.SetActiveScene(loadedScene);
            }
            else
            {
                Debug.LogError("加载的场景无效：" + sceneToAdd);
            }
        }
        else
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("PersistantLevel"));
        }

        blackBG.DOFade(0, 0.5f);
    }
}
