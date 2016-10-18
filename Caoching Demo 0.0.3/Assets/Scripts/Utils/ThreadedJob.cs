/**
* @file ThreadedJob.cs
* @brief Contains the main thread class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/

using System.Collections;

/**
* ThreadedJob class 
* @brief base class to creating communication threads
*/
public class ThreadedJob
{
    //Indicates when a thread is done
    private bool mIsDone = false;
    //Lock handle (mutex)
    private object mThreadLockHandle = new object();
    //the thread
    private System.Threading.Thread mThread = null;

    public bool IsDone
    {
        get
        {
            bool tmp;
            lock (mThreadLockHandle)
            {
                tmp = mIsDone;
            }
            return tmp;
        }
        set
        {
            lock (mThreadLockHandle)
            {
                mIsDone = value;
            }
        }
    }

    /**
    * Start()
    * @brief Starts the thread
    */
    public virtual void Start()
    {
        mThread = new System.Threading.Thread(Run);
        mThread.IsBackground = true;
        mThread.Start();
    }

    /**
    * Abort()
    * @brief Aborts the thread
    */
    public virtual void Abort()
    {
        mThread.Abort();
    }

    /**
    * ThreadFunction()
    * @brief The thread loop, overwrite this in the base class
    */
    protected virtual void ThreadFunction() { }

    /**
    * OnFinished()
    * @brief Callback when the thread is done executing
    */
    protected virtual void OnFinished() { }

    /**
    * Update()
    * @brief Updates the thread status (done or not)
    */
    public virtual bool Update()
    {
        if (IsDone)
        {
            OnFinished();
            return true;
        }
        return false;
    }

    /**
    * WaitFor()
    * @brief Wait for the thread to finish (use coroutines from Unity)
    */
    IEnumerator WaitFor()
    {
        while (!Update())
        {
            yield return null;
        }
    }

    /**
    * Run()
    * @brief Run the thread loop
    */
    private void Run()
    {
        ThreadFunction();
        IsDone = true;// once thread function is done, set it to is done
    }
}
