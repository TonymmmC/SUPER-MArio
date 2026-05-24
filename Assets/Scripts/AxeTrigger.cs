using UnityEngine;

// Coloca en el hacha al final del castillo — isTrigger = true
public class AxeTrigger : MonoBehaviour
{
    public Bowser bowser;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && bowser != null)
        {
            bowser.Defeat();
            gameObject.SetActive(false);
        }
    }
}
