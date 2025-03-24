using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace BlueStar.Inventory
{
    public class TransitionManager : Singleton<TransitionManager>
    {
        private CanvasGroup blackBG;
        public GameObject player;
        void Start()
        {
            player=GameObject.Find("Terra");
            blackBG = GameObject.Find("------UI------/UI_2D/BlackBG").gameObject.GetComponent<CanvasGroup>();
        }
    
        private void Update()
        {
            
        }
        public void Transition(String form, String to,Transform transform)
        {
            StartCoroutine(TransitionToScene(form, to,transform));
        }
    
        private IEnumerator TransitionToScene(String from, String to, Transform transform)
        {
            blackBG.DOFade(1, 0.5f);
            yield return new WaitForSeconds(1f);
            //异步加载和卸载场景
            player.transform.position=transform.position;
            yield return SceneManager.UnloadSceneAsync(from);
            yield return SceneManager.LoadSceneAsync(to,LoadSceneMode.Additive);
            
            Scene newActiveScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            //SceneManager.SetActiveScene(newActiveScene);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("PersistantLevel"));
            
            blackBG.DOFade(0, 0.5f);
        }

        public void LoadDayOne()
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("O_OverView"));
            SceneManager.LoadSceneAsync("L2_Home",LoadSceneMode.Additive);
            GameObject.Find("------Camera------/MainCamera").GetComponent<Camera>().enabled = true;
            GameObject.Find("DialogueManager").GetComponent<DialogueManager>().enabled = true;
        }


    }

}
