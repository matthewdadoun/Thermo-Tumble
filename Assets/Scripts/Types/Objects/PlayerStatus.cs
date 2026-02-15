using UnityEngine.SceneManagement;

public class PlayerStatus : DestructibleObject
{
    public override void ReactTo(ElementType other)
    {
        if (IsOpposingElement(other))
        {
            KillPlayer(true);
        }

        base.ReactTo(other);
        return;
    }

    public void KillPlayer(bool bDestroyPlayer = false)
    {
        var fadeInstance = GameManager.Instance.FadeControllerInstance;

        // Retrieve game manager instance / set up binding
        fadeInstance.OnFadeComplete += OnDeathFadeComplete;

        // Fade out to black
        fadeInstance.FadeOutToBlack(3f);

        // OnDestroyFadeComplete
        void OnDeathFadeComplete()
        {
            // Unbind event
            GameManager.Instance.FadeControllerInstance.OnFadeComplete -= OnDeathFadeComplete;

            // Reload the active scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // If we want to destroy the player, do so
        if (bDestroyPlayer)
        {
            Destroy(gameObject);
        }
    }
}