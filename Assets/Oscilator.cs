using UnityEngine;

[DisallowMultipleComponent]
public class Oscilator : MonoBehaviour
{
    [SerializeField]
    Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField]
    float period = 2f;

    float movementFactor;

    Vector3 startingPosition;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        // protects vs NaN
        if (period <= Mathf.Epsilon)
        {
            return;
        }

        float cycles = Time.time / period; // grows from 0

        const float tau = Mathf.PI * 2; // ~= 6.28
        float rawSineWave = Mathf.Sin(cycles * tau);

        movementFactor = (rawSineWave / 2f) + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}
