using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResultingForceAnimation : MonoBehaviour
{
    private class Measurement
    {
        public float time { get; set; }
        public double force { get; set; }

        public Measurement(float time, double force)
        {
            this.time = time;
            this.force = force;
        }
    }

    private Rigidbody rb;
    private float resultingForce;
    private float lastVelocity = 0;
    public bool feedback = false;

    private float previousPosition;
    private Transform hip;

    // Used to save all measurements to later be used in graph
    private List<Measurement> measurements = new List<Measurement>();
    private float startTime;

    // Measurement interval (in seconds)
    public float measurementInterval = 0.2f; // Adjust as needed
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        rb = GetComponent<Rigidbody>();

        // Recording the hip
        hip = gameObject.transform.GetChild(1);
        previousPosition = hip.position.y;
    }

    void FixedUpdate()
    {
        // Increment the timer
        // fixedDeltaTime = 0.02, FixedUpdate executes every 0.02 seconds.
        timer += Time.fixedDeltaTime;

        // Check if it's time to take a measurement
        // If timer = 0.2 it will take FixedUpdate 10 executions to run the if-statement
        if (timer >= measurementInterval)
        {
            // Reset the timer
            timer = 0;

            // Calculate velocity
            float velocity = (hip.position.y - previousPosition) / measurementInterval;
            previousPosition = hip.position.y;

            // Calculate acceleration
            float acceleration = (velocity - lastVelocity) / measurementInterval;
            lastVelocity = velocity;

            // Calculate resulting force
            resultingForce = acceleration * rb.mass;

            // calculate and format the Normal force
            double normalForce = ((rb.mass * 9.82) + resultingForce);
            normalForce = double.Parse(normalForce.ToString("0.00"));

            // Calculate the time when measurement was taken
            float currentTime = (Time.time - startTime);
            currentTime = float.Parse(currentTime.ToString("0.00"));

            // Save measurements for later use
            measurements.Add(new Measurement (currentTime, normalForce) );

            // Debug output
            if (feedback)
            {
                Debug.Log("F(R): " + resultingForce + " N || " +
                          "F(N): " + normalForce + " N || " +
                          "Spine pos: " + hip.position.y + " || " + 
                          "Velocity: " + velocity + " || " +
                          "Acceleration: " + acceleration + " || "
                          );
            }
        }
    }

    [ContextMenu("Print Measurements")]
    void SaveMeasurements()
    {
        Debug.Log(measurements[0]);
        string directoryPath = @"C:\Examensarbete\UnityCSV";
        string fileName = "unitydata_fast.csv";
        string filePath = Path.Combine(directoryPath, fileName);
        
        try
        {
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
            }

            var configMeasurements = new CsvConfiguration(CultureInfo.InvariantCulture) 
            { 
                HasHeaderRecord = true
            };

            using (StreamWriter streamWriter = new StreamWriter(filePath))
            using (CsvWriter csvWriter = new CsvWriter(streamWriter, configMeasurements))
            {
                csvWriter.WriteRecords(measurements);
            }

            Debug.Log("CSV Created successfully");

        } 
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

    }
}