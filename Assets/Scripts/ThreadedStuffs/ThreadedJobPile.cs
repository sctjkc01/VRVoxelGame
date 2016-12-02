using UnityEngine;
using System.Collections.Generic;

public class ThreadedJobPile : MonoBehaviour {
    public static ThreadedJobPile inst;
    public int QueueSize = 8;

    public List<ThreadedJob> JobsToComplete = new List<ThreadedJob>();
    private ThreadedJob[] JobsRunning;

    public static void AddJob(ThreadedJob job) {
        inst.JobsToComplete.Add(job);
        inst.JobsToComplete.Sort();
    }

    void Awake() {
        inst = this;
        JobsRunning = new ThreadedJob[QueueSize];
    }

    void Update() {
        for(int i = 0; i < JobsRunning.Length; i++) {
            if(JobsRunning[i] != null && JobsRunning[i].Update()) {
                JobsRunning[i] = null;
            }
            if(JobsRunning[i] == null && JobsToComplete.Count > 0) {
                JobsRunning[i] = JobsToComplete[0];
                JobsToComplete.RemoveAt(0);
                JobsRunning[i].Start("ThreadedJob " + i);
            }
        }
    }
}