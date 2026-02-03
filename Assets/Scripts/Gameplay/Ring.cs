using UnityEngine;

public class Ring : MonoBehaviour
{
    private float speed = 5f;
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    
    private void Update()
    {
        // Move LEFT
        transform.position += Vector3.left * speed * Time.deltaTime;
        
        // Destroy when far off-screen
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
            Debug.Log("Ring destroyed (off-screen)");
        }
    }
}