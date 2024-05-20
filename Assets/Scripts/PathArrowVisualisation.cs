using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PathArrowVisualisation : MonoBehaviour
{
    [SerializeField]
    private GameObject NavTarget;

    private NavMeshPath path;

    [SerializeField]
    private QrCodeRecenter QrCode;

    [SerializeField]
    private GameObject arrow; // Single arrow GameObject

    void Start()
    {
        path = new NavMeshPath();
    }

    void Update()
    {
        NavMesh.CalculatePath(transform.position, NavTarget.transform.position, NavMesh.AllAreas, path);

        if (path.corners.Length >= 2)
        {
            Vector3 direction = path.corners[1] - path.corners[0];
            Quaternion rotation = Quaternion.LookRotation(direction);

            //arrow.transform.position = path.corners[0]; // Move arrow to the start of the path
            arrow.transform.rotation = rotation; // Rotate arrow to align with the path direction
            arrow.SetActive(QrCode.isActivated); // Show/hide arrow based on QR code activation
        }
        else
        {
            arrow.SetActive(false); // Hide arrow if the path has less than two corners
        }
    }
}
