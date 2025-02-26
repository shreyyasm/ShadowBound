using UnityEngine;

public class CardFlip : MonoBehaviour
{
    public GameObject frontCard;
    public GameObject backCard;
    public float flipSpeed = 10f; // Adjust for desired smoothness

    public bool isFlipped = false;
    public bool isFlipping = false;
    public bool isFlippingBack = false;
    public float rotationY = 0f;
    public float targetRotation;
    float targetRotationFront = 90;
    float targetRotationBack = 0;
    bool halfWay;


    public AudioSource audioSource;
    public AudioClip FlipSFX;
    private void Update()
    {
        if (isFlipping)
        {

            rotationY = Mathf.MoveTowards(rotationY, targetRotation, Time.deltaTime * flipSpeed);

            if (isFlipped)
            {
                if(!halfWay)
                {
                    targetRotation = targetRotationFront;
                    frontCard.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
                }
                if (rotationY  >= 88) // Switch cards at halfway
                {
                    halfWay = true;
                  
                }
                if(halfWay)
                {
                    frontCard.SetActive(false);
                    backCard.SetActive(true);
                    targetRotation = targetRotationBack;
                    backCard.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
                }
            }
            

            //// Stop flipping once close to the target
            //if (Mathf.Abs(rotationY - targetRotation) < 1f)
            //{
            //    isFlipping = false;
            //}
        }
        else
        {
            if(isFlippingBack)
            {
                rotationY = Mathf.MoveTowards(rotationY, targetRotation, Time.deltaTime * flipSpeed);
                if (!halfWay)
                {
                    targetRotation = targetRotationFront;
                    backCard.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
                }
                if (rotationY >= 88) // Switch cards at halfway
                {
                    halfWay = true;

                }
                if (halfWay)
                {
                    frontCard.SetActive(true);
                    backCard.SetActive(false);
                    targetRotation = targetRotationBack;
                    frontCard.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
                }
                //if(rotationY <= 88)
                //{
                //    isFlippingBack = false;
                //}
            }

        }
    }

    public void OnHoverEnter()
    {
        if (!isFlipping && !isFlipped)
        {
            isFlipping = true;
            isFlipped = true;
            isFlippingBack = false;
            halfWay = false;
            audioSource.PlayOneShot(FlipSFX);
            
        }
    }

    public void OnHoverExit()
    {
        if (isFlipping && isFlipped)
        {
            Debug.Log("Leave");
            isFlipping = false;
            isFlippingBack = true;
            isFlipped = false;
            halfWay = false;
        }
    }
}
