using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField]
    float levelLoadDelay = 2f;
    [SerializeField]
    float rcsThrust = 68f;
    [SerializeField]
    float mainThrust = 40f;
    [SerializeField]
    AudioClip mainEngine;
    [SerializeField]
    AudioClip deathSound;
    [SerializeField]
    AudioClip levelStartSound;
    [SerializeField]
    ParticleSystem mainEngineParticles;
    [SerializeField]
    ParticleSystem successParticles;
    [SerializeField]
    ParticleSystem deathParticles;

    bool noCollisionMode = false;

    enum State
    {
        Alive,
        Dying,
        Transcending
    }

    State state = State.Alive;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (state == State.Alive)
        {
            ResponseToThrustInput();
            ResponseToRotateInput();
            if (Debug.isDebugBuild)
            {
                ResponseToDebugKeys();
            }
        }
    }

    private void ResponseToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartSuccessSequence();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            noCollisionMode = !noCollisionMode;
            print("DEBUG MODE: " + noCollisionMode);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || noCollisionMode)
        {
            return;
        }

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    { 
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelStartSound);
        successParticles.Play();
        Invoke(nameof(LoadNextScene), levelLoadDelay);
    }
    private void StartDeathSequence()
    { 
        state = State.Dying;
        audioSource.Stop();
        deathParticles.Play();
        audioSource.PlayOneShot(deathSound);
        Invoke(nameof(LoadFirstScene), levelLoadDelay);
    }
    private void ResponseToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopThrust();
        }
    }

    private void StopThrust()
    {

        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
    }

    private void ResponseToRotateInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            Rotate(rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rotate(-rotationThisFrame);
        }
    }

    private void Rotate(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false;
    }
    private void LoadNextScene()
    {
        int nextSceneIndex;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex + 1 == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // go back to first level after the last level in the build settings
        }
        else
        {
            nextSceneIndex = currentSceneIndex + 1;
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }
}
