using UnityEngine;

public class WaterBall : MonoBehaviour
{
    public EnemyDataRange enemyDataRange;

    public float damage;
    public float lifeTime;
    public float homingSpeed = 2f; // ���� �ӵ�
    public float homingDuration = 2f; // ���� ���� �ð�
    private Player player;
    private Rigidbody rigidbody;
    private bool isHomingActive = true; // ���� Ȱ��ȭ ����
    private float homingTimer = 0f; // ���� �ð��� ������ Ÿ�̸�

    private void Start()
    {
        damage = enemyDataRange.f_autoAttackRangedDamage;
        player = GameObject.FindWithTag("PlayerScript").GetComponent<Player>();
        lifeTime = enemyDataRange.f_deleteTime;
        Destroy(gameObject, lifeTime); // ���� �ð� �� ����

        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Time.deltaTime�� ����Ͽ� homingDuration�� ������ ���� ����� ��Ȱ��ȭ
        if (isHomingActive)
        {
            homingTimer += Time.deltaTime;
            if (homingTimer >= homingDuration)
            {
                isHomingActive = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isHomingActive && player != null && !player.CheckDie())
        {
            // �÷��̾�� ������ y���� �����ϰ�, xz�� �������� ����
            Vector3 targetPosition = player.transform.position;
            targetPosition.y = transform.position.y; // �߻�ü�� y�� ����

            // XZ������ ����
            Vector3 directionToPlayer = (targetPosition - transform.position).normalized;
            Vector3 newDirection = Vector3.Lerp(rigidbody.velocity.normalized, directionToPlayer, homingSpeed * Time.fixedDeltaTime).normalized;

            rigidbody.velocity = newDirection * rigidbody.velocity.magnitude; // �ӷ� ������ ä�� ���� ����
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (player != null && !player.CheckDie())
            {
                var message = new DamageMessage();
                message.amount = damage;
                player.ApplyDamage(message);
            }
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Default") || other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
