using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public struct LocalPosition
{
    public int x;
    public int y;

    public LocalPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }    
}

[System.Serializable]
public struct WorldPosition
{
    public float x;
    public float y;
    public float z;

    public WorldPosition(float x, int y, float z) : this()
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

public class Unit : MonoBehaviour {
    private const float ATTACK_ADV_MULT = 0.05f;
    private const float DEFENSE_ADV_MULT = 0.0025f;
    private const int NOT_INITIALIZED_PARAM = int.MinValue;

    public LocalPosition localPosition;
    public WorldPosition worldPosition;
    public bool isBotUnit;
    public int attackSkill;
    public int defenseSkill;
    public int damageMin;
    public int damageMax;
    public int health;
    public int distance;
    public int countOfWarrior;
    public int shot; // only for bowman
    public float damageResult;
    public int teamDamage = NOT_INITIALIZED_PARAM;
    public int teamHealth = NOT_INITIALIZED_PARAM;
    public int lastWarriorDamage = 0;
    
    public List<Node> currentPath;
    public List<Node> currentPathToEnemy;

    public Color startColor;
    public Color availableTileColor;
    public List<Node> availableMovementTiles = null;

    public void UpdatePosition(WorldPosition worldPosition)
    {
        this.worldPosition = worldPosition;
        this.localPosition = PositionConverter.ToLocalCoordinates(worldPosition);
    }

    public void MoveToEnterTile()
    {
        if (currentPath != null)
        {
            WorldPosition moveTo = PositionConverter.ToWorldCoordinates(new LocalPosition(currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].y));
            moveTo.y = transform.position.y;
            //StartCoroutine(MoveObject.MoveUnit(moveTo));
            transform.position = moveTo.ToVector3();
            UpdatePosition(moveTo);
        }
    }

    public static bool isHumanMakeTurn = false;

    public int DistanceTo(Unit unit)
    {
        int x0 = localPosition.x - Mathf.FloorToInt(localPosition.y / 2);
        int y0 = localPosition.y;
        int x1 = unit.localPosition.x - Mathf.FloorToInt(unit.localPosition.y / 2);
        int y1 = unit.localPosition.y;
        int dx = x1 - x0;
        int dy = y1 - y0;

        return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy), Mathf.Abs(dx + dy));
    }

    public bool CanAttack(Unit enemy)
    {
        return DistanceTo(enemy) <= distance;
    }

    public void Approach()
    {
        if (currentPathToEnemy != null)
        {
            WorldPosition moveTo = PositionConverter.ToWorldCoordinates(new LocalPosition(
                currentPathToEnemy[currentPathToEnemy.Count - 1].x, 
                currentPathToEnemy[currentPathToEnemy.Count - 1].y));
            moveTo.y = transform.position.y;
            //StartCoroutine(MoveObject.MoveUnit(moveTo));
            transform.position = moveTo.ToVector3();
            UpdatePosition(moveTo);
        }
    }

    public void Attack(Unit enemy)
    {
        Approach();
        MakeDamage(enemy);
    }

    private void MakeDamage(Unit enemy)
    {
        this.InitTeamHealthIfNotInitialized();
        enemy.InitTeamHealthIfNotInitialized();

        CalcDamageResult(enemy);
        teamDamage = (int)(damageResult * countOfWarrior);

        enemy.teamHealth -= teamDamage;

        enemy.RecountWarriors();

        if (enemy.countOfWarrior < 1)
        {
            if (enemy.isBotUnit)
            {
                BoardManager.Instance.enemyUnits.Remove(enemy);
                Bot.botTeam.Remove(enemy); //todo: refactor
            }
            else
            {
                BoardManager.Instance.playerUnits.Remove(enemy);
                Human.humanTeam.Remove(enemy); //todo: refactor
            }
            Destroy(enemy.gameObject);            
        }
    }

    private void InitTeamHealthIfNotInitialized()
    {
        if (teamHealth == NOT_INITIALIZED_PARAM)
        {
            teamHealth = health * countOfWarrior - lastWarriorDamage;
        }
    }

    private void CalcDamageResult(Unit enemy)
    {
        System.Random rand = new System.Random();
        damageResult = rand.Next(damageMin, damageMax + 1);
        if (attackSkill > enemy.defenseSkill)
        {
            damageResult += damageResult * ATTACK_ADV_MULT;
        }
        else if (attackSkill < enemy.defenseSkill)
        {
            damageResult -= damageResult * DEFENSE_ADV_MULT;
        }        
    }

    private void RecountWarriors()
    {
        countOfWarrior = teamHealth / health;
        lastWarriorDamage = health - teamHealth % health;
    }    
}
