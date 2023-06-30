using System.Collections;
using System.Collections.Generic;
using Google.Play.Review;
using UnityEngine;

public class AndroidReview : MonoBehaviour {

    private ReviewManager _myReviewManager;
    //private PlayReviewInfo _playReviewInfo;
    private string errorMessage;
    private bool IsTryingToAskForReview;
    private PlayReviewInfo _myPlayReviewInfo;

    void Start()
    {
       StartCoroutine(requestFlowOperationRoutine());
    }

    IEnumerator requestFlowOperationRoutine()
    {
        Debug.Log("requestFlowOperationRoutine");

        _myReviewManager = new ReviewManager();
        var requestFlowOperation = _myReviewManager.RequestReviewFlow();
        yield return requestFlowOperation;

        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log("review info error: " + requestFlowOperation.Error.ToString());
            IsTryingToAskForReview = false;
            yield break;
        }
        _myPlayReviewInfo = requestFlowOperation.GetResult();

        StartCoroutine(launchFlowOperationRoutine());
    }

    IEnumerator launchFlowOperationRoutine()
    {
        Debug.Log("launchFlowOperationRoutine");

        var launchFlowOperation = _myReviewManager.LaunchReviewFlow(_myPlayReviewInfo);
        yield return launchFlowOperation;

        _myPlayReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.Log("review launch error: " + launchFlowOperation.Error.ToString());
            IsTryingToAskForReview = false;
            yield break;
        }

        yield return null;
        IsTryingToAskForReview = false;
    }

}
