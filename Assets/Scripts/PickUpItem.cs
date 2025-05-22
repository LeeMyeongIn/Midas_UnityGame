using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    Transform player;
    [SerializeField] float speed = 5f;
    [SerializeField] float pickUpDistance = 1.5f;
    [SerializeField] float ttl = 10f;

    public Item item;
    public int count = 1;


    private void Awake()
    { 
        player = GameManager.instance.player.transform;
    }

    public void Set(Item item, int count)
    {
        this.item = item;
        this.count = count;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = item.icon;
    }

    private void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl < 0) { Destroy(gameObject); }

        float distance = Vector3.Distance(transform.position, player.position);

    
        if (distance > pickUpDistance)
        {
            return;
        }
       
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
            );

        if (distance < 0.1f)
        {   
            //여기보단 다른 구체적인 컨트롤러로 옮겨야 함
            if(GameManager.instance.inventoryContainer != null)
            {
                GameManager.instance.inventoryContainer.Add(item, count);
                CropSeenManager.Instance?.RegisterSeenItem(item.id);
            }
            else
            {
                Debug.LogWarning("NO INVENTORY CONTAINER ATTACHED TO THE GAME MANAGER");
            }
            Destroy(gameObject);
        }
    }
}
