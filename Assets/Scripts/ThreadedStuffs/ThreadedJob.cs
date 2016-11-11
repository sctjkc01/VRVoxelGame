using System;
using System.Collections;

public class ThreadedJob : IComparable<ThreadedJob> {
    private bool m_Done = false;
    private object m_Handle = new object();
    private System.Threading.Thread m_Thread = null;
    public bool IsDone {
        get {
            bool tmp;
            lock (m_Handle) {
                tmp = m_Done;
            }
            return tmp;
        }
        set {
            lock (m_Handle) {
                m_Done = value;
            }
        }
    }
    private float m_Priority = -1f;
    public float Priority {
        get {
            float tmp;
            lock(m_Handle) {
                tmp = m_Priority;
            }
            return tmp;
        }
        set {
            lock(m_Handle) {
                m_Priority = value;
            }
        }
    }

    /// <summary>
    /// Initialization of job, performed on Unity thread
    /// </summary>
    public virtual void Start() {
        m_Thread = new System.Threading.Thread(Run);
        m_Thread.Start();
    }
    /// <summary>
    /// Early termination of job
    /// </summary>
    public virtual void Abort() {
        m_Thread.Abort();
    }

    /// <summary>
    /// Main processing of job, performed on separate thread
    /// </summary>
    protected virtual void ThreadFunction() { }

    /// <summary>
    /// Final processing of job, performed on Unity thread
    /// </summary>
    protected virtual void OnFinished() { }

    /// <summary>
    /// Update function, called on Unity thread to check if it's complete.
    /// </summary>
    /// <returns>Did this job complete?</returns>
    public virtual bool Update() {
        if(IsDone) {
            OnFinished();
            return true;
        }
        return false;
    }
    /// <summary>
    /// Basic "Wait for this Job to finish" for coroutine use
    /// </summary>
    public IEnumerator WaitFor() {
        while(!Update()) {
            yield return null;
        }
    }
    /// <summary>
    /// Basic execution of job, performed on separate thread
    /// </summary>
    private void Run() {
        ThreadFunction();
        IsDone = true;
    }

    public int CompareTo(ThreadedJob other) {
        return Priority.CompareTo(other.Priority);
    }
}