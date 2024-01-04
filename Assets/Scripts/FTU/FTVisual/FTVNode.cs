using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class FTVNode : MonoBehaviour
{
    private FTEntity entity;
    private FTAgent agent;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private TMP_Text nameBox,goalBox, actionBox;
    [SerializeField]
    private GameObject connectionsContainer, connection;
    public List<DistanceJoint2D> joints;
    [SerializeField]
    private Color colorLocation, colorCharacter, colorItem, colorOther;



    private void Start()
    {
        if (GetComponent<FTAgent>())
        {
            agent = GetComponent<FTAgent>();
        }
        entity = GetComponent<FTEntity>();
        spriteRenderer = GetComponent<SpriteRenderer>();


        switch (entity.entityState.entityType)
        {
            case FTEntityType.Location:
                spriteRenderer.color = colorLocation;//new Color(96, 70, 59);
                break;
            case FTEntityType.Character:
                spriteRenderer.color = colorCharacter;
                break;
            case FTEntityType.Item:
                spriteRenderer.color = colorItem;
                break;
            case FTEntityType.Other:
                spriteRenderer.color = colorOther;
                break;
        }

        nameBox.text = name;
        RegenerateConnections();
        joints = new List<DistanceJoint2D>();
        GenerateJoints();
    }

    private void RegenerateConnections()
    {
        DestroyConnections();
        GenerateConnections();
    }

    private void DestroyConnections()
    {
        foreach (LineRenderer c in connectionsContainer.transform.GetComponentsInChildren<LineRenderer>())
        {
            Destroy(c.gameObject);
        }
    }

    private void GenerateConnections()
    {
        List<FTEntity> toRemove = new List<FTEntity>();
        foreach (FTEntity e in entity.connectedEntities)
        {
            if (e == null)
            {
                toRemove.Add(e);
                break;
            }
            if (!e.gameObject)
            {
                toRemove.Add(e);
                break;
            }
            AddConnection(e.gameObject);
        }
        toRemove.ForEach(x => entity.connectedEntities.Remove(x));
        
    }

    private void AddConnection(GameObject target)
    {
        GameObject newConnection = Instantiate(connection, connectionsContainer.transform);
        newConnection.GetComponent<LineRenderer>().SetPositions(new[] { connectionsContainer.transform.position, 
            target.transform.Find("Connections").transform .position} );
    }


    public void AddJoint(GameObject e, bool reverse = false)
    {
        DistanceJoint2D j = gameObject.AddComponent(typeof(DistanceJoint2D)) as DistanceJoint2D;
        j.autoConfigureDistance = false;
        j.distance = 6;
        j.enableCollision = true;
        j.connectedBody = e.GetComponent<Rigidbody2D>();
        joints.Add(j);

        if (!reverse) e.GetComponent<FTVNode>().AddJoint(gameObject, true);
    }

    public void RemoveJoint(GameObject e, bool reverse = false)
    {
        List<DistanceJoint2D> toDestroy = new List<DistanceJoint2D>();

        foreach (DistanceJoint2D j in joints)
        {
            if (j.connectedBody = e.GetComponent<Rigidbody2D>())
            {
                toDestroy.Add(j);
            }
        }

        foreach (DistanceJoint2D j in toDestroy)
        {
            joints.Remove(j);
            Destroy(j);
        }

        if(!reverse) e.GetComponent<FTVNode>().RemoveJoint(gameObject, true);
    }

    private void GenerateJoints()
    {
        foreach (FTEntity e in entity.connectedEntities)
        {
            AddJoint(e.gameObject, true);
        }
    }

    private void OnMouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
        


    private void Update()
    {
        RegenerateConnections();
        if (agent == null)
        {
            actionBox.text = "";
            goalBox.text = "";
        }

        else
        {
            actionBox.text = "Action: " + agent.GetCurrentActionName();
            goalBox.text = "Goal: " + agent.GetCurrentGoalName();
        }
    }
}
