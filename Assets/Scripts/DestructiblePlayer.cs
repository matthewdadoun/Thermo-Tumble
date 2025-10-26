using UnityEngine.SceneManagement;

public class DestructiblePlayer : DestructibleObject
{
    public override void ReactTo(ElementType other)
    {
        if (IsOpposingElement(other))
        {
            var fadeInstance = GameManager.Instance.FadeControllerInstance;
            
            // Retrieve game manager instance / set up binding
            fadeInstance.OnFadeComplete += OnDestroyFadeComplete;
            
            // Fade out to black
            fadeInstance.FadeOutToBlack(3f);
        }

        base.ReactTo(other);
        return;

        // OnDestroyFadeComplete
        void OnDestroyFadeComplete()
        {
            // Unbind event
            GameManager.Instance.FadeControllerInstance.OnFadeComplete -= OnDestroyFadeComplete;
            
            // Reload the active scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}