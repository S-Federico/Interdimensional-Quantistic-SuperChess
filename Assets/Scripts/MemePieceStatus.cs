using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MemePieceStatus : MonoBehaviour
{
    public PieceType PieceType;
    public int Hp = 1;
    public int Attack = 1;
    public PieceColor PieceColor;
    private NavMeshAgent navMeshAgent;
    private MemePieceStatus currentTarget;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log($"Piece {gameObject.name} loaded");
        navMeshAgent = GetComponent<NavMeshAgent>();
        DetectTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget) {
            navMeshAgent.destination = currentTarget.transform.position;
        } else {
            DetectTarget();
        }

        if (this.Hp <= 0) {
            Destroy(gameObject);
        }
    }

    private void DetectTarget() {
        MemePieceStatus[] enemies = FindObjectsByType<MemePieceStatus>(FindObjectsSortMode.None);
        enemies = enemies.Where(e => e.PieceColor != this.PieceColor).ToArray();

        if (enemies == null || enemies.Length == 0) {
            Debug.Log($"Il pezzo {gameObject.name} non ha trovato nemici");
            return;
        }
        
        // Select first enemy detected as current enemy
        currentTarget = enemies.First();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{gameObject.name} colliding with {collision.gameObject.name}");
        // ContactPoint contact = collision.contacts[0];
        // Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        // Vector3 position = contact.point;
        // Instantiate(explosionPrefab, position, rotation);
        // Destroy(gameObject);
        MemePieceStatus colliderPieceStatus = collision.gameObject.GetComponent<MemePieceStatus>();
        if (colliderPieceStatus != null) {
            if (colliderPieceStatus.PieceColor != this.PieceColor) {
                colliderPieceStatus.Damage(this.Attack);
            }
        }
    }

    public void Damage(int damage) {
        this.Hp -= damage;
    }

    
}
