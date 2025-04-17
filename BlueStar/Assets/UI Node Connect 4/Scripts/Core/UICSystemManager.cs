using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MeadowGames.UINodeConnect4
{
    [DefaultExecutionOrder(-20)]
    [ExecuteInEditMode]
    public class UICSystemManager : MonoBehaviour
    {
        [SerializeField] static UICSystemManager _instance;
        public static UICSystemManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UICSystemManager>();
                }
                return _instance;
            }
            set => _instance = value;
        }

        [SerializeField] List<Node> _nodes = new List<Node>();
        public static List<Node> Nodes
        {
            get
            {
                if (Instance && Instance._nodes == null)
                    Instance._nodes = new List<Node>();
                return Instance?._nodes;
            }
        }

        [SerializeField] List<Connection> _connections = new List<Connection>();
        public static List<Connection> Connections { get => Instance ? Instance._connections : new List<Connection>(); }
        //已经连接的NodePair的列表
        [SerializeField] List<NodePair> _ExistNodePairs = new List<NodePair>();
        public static List<NodePair> ExistNodePairs
        {
            get => Instance ? Instance._ExistNodePairs : new List<NodePair>();
        }
        
        //封装一个生产线的List，在List外面再包装一层作为一个生产线的列表
        [System.Serializable]
        public class NodePairListWrapper
        {
            public List<NodePair> nodePairs = new List<NodePair>();
        }
        [SerializeField] private List<NodePairListWrapper> _ManufactureLine = new List<NodePairListWrapper>();

        public static List<NodePairListWrapper> ManufactureLine =>
            Instance ? Instance._ManufactureLine : new List<NodePairListWrapper>();

        [SerializeField] bool _cacheRaycasters = true;
        public bool CacheRaycasters
        {
            get => _cacheRaycasters;
            set
            {
                raycasterList = new List<GraphicRaycaster>();
                if (value == true)
                {
                    raycasterList.AddRange(FindObjectsOfType<GraphicRaycaster>());
                }
                _cacheRaycasters = value;
            }
        }
        public static List<GraphicRaycaster> raycasterList = new List<GraphicRaycaster>();
        
        /// <summary>
        /// 比较已经连好的生产线和某个正确的生产线
        /// </summary>
        /// <param name="想要生产的链路的Index"></param>
        public static void CompareSteps(int index)
        {
            if (index<=ManufactureLine.Count-1)
            {
                foreach (var pair in ManufactureLine[index].nodePairs)
                {
                    if (!ExistNodePairs.Contains(pair))
                    {
                        Debug.Log("生产链路不完整");
                        break;
                    }
                
                }

                if (ManufactureLine[index].nodePairs.TrueForAll(pair =>ExistNodePairs.Contains(pair)))
                {
                    Debug.Log("链路完整，开始执行生产");
                }
            }
        }
        public static void AddNodeToList(Node node)
        {
            if (Instance && !Nodes.Contains(node))
            {
                Nodes.Add(node);
                UICEvents.TriggerEvent(UICEventType.NodeAdded, node);
            }
        }

        public static void RemoveNodeFromList(Node node)
        {
            if (Instance && Nodes.Contains(node))
            {
                Nodes.Remove(node);
                UICEvents.TriggerEvent(UICEventType.NodeRemoved, node);
            }
        }

        public static void AddConnectionToList(Connection connection)
        {
            if (!Connections.Contains(connection))
            {
                Connections.Add(connection);
                //新加的
                ExistNodePairs.Add(connection.NodePair);
                UICEvents.TriggerEvent(UICEventType.ConnectionAdded, connection);
 
                // v4.1 - bugfix: - connection line position not updating when added using UICSystemManager.AddConnectionToList 
                connection.UpdateLine(true);
            }
        }

        public static void RemoveConnectionFromList(Connection connection)
        {
            if (Connections.Contains(connection))
            {
                Connections.Remove(connection);
                //新加的
                ExistNodePairs.Remove(connection.NodePair);
                UICEvents.TriggerEvent(UICEventType.ConnectionRemoved, connection);
            }
        }

        // list of selected elements, used for single or multi selection
        public static List<ISelectable> selectedElements = new List<ISelectable>();
        public static IElement clickedElement;
        public static IElement hoverElement;

        static EventManager<IElement> _uicEvents;
        public static EventManager<IElement> UICEvents
        {
            get
            {
                if (_uicEvents == null)
                    _uicEvents = new EventManager<IElement>();

                return _uicEvents;
            }
        }

        public static void UpdateNodeList()
        {
            UICSystemManager.Instance._nodes = new List<Node>();
            Nodes.AddRange(FindObjectsOfType<Node>());
        }

        void OnEnable()
        {
            CacheRaycasters = _cacheRaycasters;

            selectedElements = new List<ISelectable>();

            // ensures single instance
            if (Instance != this)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    GameObject.Destroy(gameObject);
#if UNITY_EDITOR
                else
                    GameObject.DestroyImmediate(gameObject);
#endif
            }

            UpdateNodeList();
        }

        public static List<GraphManager> graphManagers = new List<GraphManager>();

        void Start()
        {
            // initialize the editor mode connections
            for (int i = 0; i < Connections.Count; i++)
            {
                Connections[i].UpdateLine();
                Connections[i].OnPointerUp();
            }

            ExistNodePairs.Clear();
            
            for (int i = 0; i < Connections.Count; i++)
            {
                ExistNodePairs.Add(Connections[i].NodePair);
            }
        }

        void Update()
        {
            e_OnUpdate.Invoke();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                for (int i = 0; i < Connections.Count; i++)
                {
                    Connections[i].UpdateLine();
                }
            }
#endif
        }

        static UnityEvent e_OnUpdate = new UnityEvent();
        static List<UnityAction> actions = new List<UnityAction>();

        public static void AddToUpdate(UnityAction action)
        {
            if (!actions.Contains(action))
            {
                e_OnUpdate.AddListener(action);
                actions.Add(action);
            }
        }
        public static void RemoveFromUpdate(UnityAction action)
        {
            if (actions.Contains(action))
            {
                e_OnUpdate.RemoveListener(action);
                actions.Remove(action);
            }
        }
    }
}