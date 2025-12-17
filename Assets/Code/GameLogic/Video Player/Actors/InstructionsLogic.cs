using TestProject;
using TMPro;
using UnityEngine;

public class InstructionsLogic : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Number of seconds to display the instructions")]
    public float displayDuration = 7.0f;

    private float hangTimer = 0.0f;
    private TMP_Text instructionText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instructionText = GetComponent<TMP_Text>();

        if (instructionText == null)
        {
            Utils.LogErrorMessage("Couldn't find the instruction text component!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        var progress = 1 - (hangTimer / displayDuration);
        if (hangTimer <= displayDuration)
        {
            instructionText.alpha = Mathf.Lerp(0.0f, 1, progress);
        }

        hangTimer += Time.deltaTime;
    }
}
