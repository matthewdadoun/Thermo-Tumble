using UnityEngine;

public class DestructibleObject : ElementalBehaviour, IDestructible
{
    private bool _bIsHeld = false;

    public void OnDestructibleOverlap(GameObject destroyer)
    {
        // If destroyer is valid
        if (destroyer)
        {
            // Destroy this game object
            Destroy(gameObject);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        var elementalBehaviour = other.gameObject.GetComponent<ElementalBehaviour>();
        if (elementalBehaviour == null)
        {
            return;
        }

        // Try to retrieve player status
        var playerStatus = other.gameObject.GetComponent<PlayerStatus>();
        if (_bIsHeld && playerStatus != null)
        {
            return;
        }

        // Retrieve player input handler
        var playerInputHandler = gameObject.GetComponent<PlayerInputHandler>();
        
        // Check to see if player input handler is null
        if (playerInputHandler != null)
        {
            // If this input handler is holding a grabbable, don't react to this object
            if (playerInputHandler.IsHoldingGrabbable())
            {
                return;
            }
        }

        // Have both objects react to each other
        elementalBehaviour.ReactTo(element);
        ReactTo(elementalBehaviour.Element);
    }

    public override void ReactTo(ElementType other)
    {
        if (IsOpposingElement(other))
        {
            var explosionInstance = ElementalExplosions.Instance;

            // Check to see which explosion to use
            foreach (var elementExplosion in explosionInstance.elementExplosions)
            {
                // Cause an explosion
                if (elementExplosion.element == element)
                {
                    Instantiate(elementExplosion.explosionObject, transform.position, Quaternion.identity);
                    break;
                }

                // Retrieve sound manager
                var sm = SoundManager.Instance;

                // If sound manager is null, 
                if (sm == null)
                {
                    continue;
                }
                
                // Play the correct sound
                var sound = Element == ElementType.Lava ? sm.sfxFire : sm.sfxIce;
                sm.PlaySfx(sound, 0.25f);
            }

            // Retrieve player input handler
            var playerInputHandler = gameObject.GetComponent<PlayerInputHandler>();
        
            // Check to see if player input handler is null
            if (playerInputHandler != null)
            {
                // Stop music
                SoundManager.Instance.StopMusic();
                SoundManager.Instance.PlaySfx(SoundManager.Instance.sfxFail);
            }
       
            
            Destroy(gameObject);
        }
    }

    public void SetIsHeld(bool bHeld)
    {
        // Set held state
        _bIsHeld = bHeld;
    }
}