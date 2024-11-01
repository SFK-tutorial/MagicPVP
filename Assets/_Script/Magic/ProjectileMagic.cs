using System.Collections;
using System.Collections.Generic;
using Mirror;
using ParrelSync;
using UnityEngine;

public class ProjectileMagic : Magic
{
    [SerializeField]
    GameObject _projectilePrefab;

    [SerializeField]
    Transform _generatePos;

    Vector2 _direction;
    
    protected override void MagicStart()
    {
        GenerateProjectile(_generatePos.position, Vector2.right);

    }
    [ClientRpc]
    void PlayAnimation()
    {
        
    }
    

    [Command]
    public void GenerateProjectile(Vector2 pos, Vector2 dir)
    {
        var go = Instantiate(_projectilePrefab);
        go.transform.position = pos;
        go.transform.right = dir;
        NetworkServer.Spawn(go);
        var p = go.GetComponent<Projectile2D>();
        p.Lauch(dir);
    }
}
