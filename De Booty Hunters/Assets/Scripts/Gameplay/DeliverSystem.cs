using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliverSystem : MonoBehaviour
{
    public Transform[] deliveryPoints; // holds delivery points
    public float deliveryDistance = 2f; // distance at which a delivery is considered complete
    public TextMeshProUGUI deliveryStatusText; // UI text to display delivery status
    private Transform currentDeliveryPoint;
    private bool hasDelivery;

    private void Start()
    {
        SelectNewDeliveryPoint();
    }

    private void Update()
    {
        UpdateDeliveryStatusText();

        if (hasDelivery && Vector3.Distance(transform.position, currentDeliveryPoint.position) < deliveryDistance)
        {
            CompleteDelivery();
        }
    }
    private void SelectNewDeliveryPoint()
    {
        int index = Random.Range(0, deliveryPoints.Length);
        currentDeliveryPoint = deliveryPoints[index];
        hasDelivery = true;
    }
    private void CompleteDelivery()
    {
        hasDelivery = false;

        SelectNewDeliveryPoint();
    }
    private void UpdateDeliveryStatusText()
    {
        if (hasDelivery)
        {
            deliveryStatusText.text = "Delivery: " + currentDeliveryPoint.name;
        }
        else
        {
            deliveryStatusText.text = "No active delivery";
        }
    }
}
