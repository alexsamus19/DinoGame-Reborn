using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip _collectSound;
    //[SerializeField] private ParticleSystem _collectEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
{
    AudioSource.PlayClipAtPoint(_collectSound, transform.position);
    //Instantiate(_collectEffect, transform.position, Quaternion.identity);

    GameManager.Instance.AddCoin();

    Destroy(gameObject);
}
}