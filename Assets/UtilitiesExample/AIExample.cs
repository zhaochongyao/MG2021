using UnityEngine;
using Utilities.AI;

namespace UtilitiesExample
{
    /// <summary>
    /// AI样例
    /// </summary>
    public class AIExample : MonoBehaviour
    {
        // 状态机
        private StateMachine<AIExample> _stateMachine;

        private Rigidbody _rigidbody;

        private MeshRenderer _meshRenderer;

        [SerializeField] private Material _playerMaterial;
    
        [SerializeField] private Material _enemyMaterial;

        private Transform _target;
    
        [SerializeField] private float _moveSpeed;

        [SerializeField] private float _sightRadius;

        private Vector3 _toTarget;

        private bool _playerRage;

        private void Awake()
        {
            InitComponent();
            InitStateMachine();
            Debug.Log("Awake");
            _toTarget = Vector3.zero;
        }

        private void OnEnable()
        {
            // 每次从对象池取出时，重启状态机
            Debug.Log("OnEnable");
            _stateMachine.Boot();
        }

        private void Start()
        {
            Debug.Log("Start");
        }

        private void InitComponent()
        {
            _target = FindObjectOfType<Player>().transform;
            _rigidbody = GetComponent<Rigidbody>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void InitStateMachine()
        {
            // 初始化状态机
            _stateMachine = new StateMachine<AIExample>();

            // 初始化所有状态
            Run run = new Run(this);
            Chase chase = new Chase(this);
            Idle idle = new Idle(this);
            Death death = new Death(this);

            // 定义所有过渡条件
            bool WithinRange() => _toTarget.magnitude < _sightRadius;
            bool OutOfRange() => !WithinRange();
            bool InDanger() => _playerRage && WithinRange();
            bool Safe() => !InDanger();
            bool GoDie() => _playerRage && Input.GetKey(KeyCode.Tab);

            // 设置初态
            _stateMachine.DefaultState = idle;
        
            // 添加所有过渡边
            _stateMachine.AddTransition(idle, chase, WithinRange);
            _stateMachine.AddTransition(chase, idle, OutOfRange);
            _stateMachine.AddTransition(idle, run, InDanger);
            _stateMachine.AddTransition(chase, run, InDanger);
            _stateMachine.AddTransition(run, idle, Safe);
        
            _stateMachine.AddAnyTransition(death, GoDie);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _playerRage = !_playerRage;
            } 
            else if (Input.GetKey(KeyCode.Space))
            {
                _stateMachine.Stop();
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _stateMachine.Resume();
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                _stateMachine.Boot();
            }
        

            _toTarget = _target.position - transform.position;
        
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, _sightRadius);
        }

        private sealed class Run : State<AIExample>
        {
            public Run(AIExample owner) : base(owner) { }

            public override void OnFixedUpdate()
            {
                Vector3 dir = self._toTarget.normalized * -1;
                self._rigidbody.MovePosition(self.transform.position
                                             + Time.fixedDeltaTime * self._moveSpeed * dir);
            }
        }

        private sealed class Chase : State<AIExample>
        {
            public Chase(AIExample owner) : base(owner) { }

            public override void OnFixedUpdate()
            {
                Vector3 dir = self._toTarget.normalized;
                self._rigidbody.MovePosition(self.transform.position
                                             + Time.fixedDeltaTime * self._moveSpeed * dir);
            }
        }
        
        private sealed class Idle : State<AIExample>
        {
            public Idle(AIExample owner) : base(owner) { }

            public override void OnEnter()
            {
                self._meshRenderer.material = self._playerMaterial;
            }

            public override void OnExit()
            {
                self._meshRenderer.material = self._enemyMaterial;
            }
        }
        
        private sealed class Death : State<AIExample>
        {
            public Death(AIExample owner) : base(owner) { }

            public override void OnEnter()
            {
                self.transform.localScale *= 2f;
            }
        }
    }
}