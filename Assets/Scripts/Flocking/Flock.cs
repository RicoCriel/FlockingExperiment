using UnityEngine;

public class Flock : MonoBehaviour
{
    public float Speed;
    private FlockingManager _flockManager;
    private Bounds _fishTankBounds;

    private void Start()
    {
        _flockManager = FlockingManager.Instance;
        if(_flockManager != null)
        {
            Speed = Random.Range(_flockManager.MinSpeed, _flockManager.MaxSpeed);
        }

        _fishTankBounds = new Bounds(_flockManager.transform.position, _flockManager.SwimLimits * 2);
    }

    private void Update()
    {
        UpdateBehaviour();
    }
    private void UpdateBehaviour()
    {
        if (!IsWithinBounds())
        {
            RotateTowardsCenter();
        }
        else
        {
            RecalculateSpeed();

            //Seperate in different factors in FlockingManager
            if (Random.Range(0, 100) < 10)
            {
                ApplyFlockingRules();
            }
        }

        //Move the fish forward
        this.transform.Translate(0, 0, Speed * Time.deltaTime);
    }

    private void RotateTowardsCenter()
    {
        //Turn around back to the center of the tank
        Vector3 direction = _flockManager.transform.position - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction),
                                                                            _flockManager.RotationSpeed * Time.deltaTime);
    }

    private bool IsWithinBounds()
    {
        return _fishTankBounds.Contains(this.transform.position);
    }

    private void RecalculateSpeed()
    {
        if (Random.Range(0, 100) < 10)
        {
            Speed = Random.Range(_flockManager.MinSpeed, _flockManager.MaxSpeed);
        }
    }

    private void ApplyFlockingRules()
    {
        if(_flockManager == null)
        {
            return;
        }

        var allFish = _flockManager.AllFish;

        Vector3 groupCenter = Vector3.zero;
        Vector3 avoidDirection = Vector3.zero;
        float groupSpeed = 0.01f;
        float neighbourDistance;
        int groupSize = 0;

        foreach(var fish in allFish)
        {
            if(fish != this.gameObject)
            {
                neighbourDistance = (fish.transform.position - this.transform.position).sqrMagnitude;
                if(neighbourDistance <= _flockManager.NeighbourDistance)
                {
                    groupCenter += fish.transform.position;
                    groupSize++;

                    if(neighbourDistance < 1.0f)
                    {
                        avoidDirection = avoidDirection + (this.transform.position - fish.transform.position);
                    }

                    if(fish.TryGetComponent<Flock>(out Flock anotherFlock))
                    {
                        groupSpeed = groupSpeed + anotherFlock.Speed;
                    }
                }
            }
        }

        if(groupSize > 0)
        {
            groupCenter = groupCenter / groupSize + (_flockManager.GoalPosition - this.transform.position);
            Speed = groupSpeed / groupSize;
            Speed = Mathf.Min(Speed, _flockManager.MaxSpeed);

            Vector3 direction = (groupCenter + avoidDirection) - this.transform.position;

            if(direction != Vector3.zero)
            {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, 
                                                                                    Quaternion.LookRotation(direction),
                                                                                    _flockManager.RotationSpeed * Time.deltaTime);
            }
        }

    }

    //private Vector3 ApplyAlignment()
    //{

    //}

    //private Vector3 ApplyCohesion()
    //{

    //}

    //private Vector3 ApplySeperation()
    //{

    //}

    //private Vector3 ApplyAvoidance()
    //{

    //}

    
}
