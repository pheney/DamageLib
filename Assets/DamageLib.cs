using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 2017-8-9
/// General library for managing recurring damage.
/// 
/// Usage: Called when a projectile impacts a target, setting the target on fire.
/// 
/// public void OnColliderEnter(Collision impact) {
///     GameObject target = impact.collider.gameObject;
///     DamageLib.AddDamageToTarget(target, dmg, rate, duration, (int)DamageType.Fire);
/// }
/// 
/// Usage: Called when entering or exiting a poisonous area
/// 
/// public void OnTriggerEnter(Trigger trespasser) {
///     GameObject target = trespasser.gameObject;
///     DamageLib.ZoneDamageStart(target, dmg, rate, (int)DamageType.Poison);
/// }
/// 
/// public void OnTriggerExit(Trigger trespasser) {
///     GameObject target = trespasser.gameObject;
///     DamageLib.ZoneDamageEnd(target, dmg, rate, (int)DamageType.Poison);
/// }  
/// </summary>
public static class DamageLib
{
    #region Data

    /// <summary>
    /// 2017-8-9
    /// Key: Hashcode of the target GameObject
    /// Value: DamageList object that contains all the 'on-going damage' for the target.
    /// </summary>
    private static Dictionary<int, DamageList> damageIndex;

    #endregion
    #region Public API

    /// <summary>
    /// 2017-8-10
    /// Removes all on-going damage for all targets. Clears all damage lists.
    /// Returns the number of entries removed.
    /// </summary>
    public static int Clear()
    {
        int result = 0;
        foreach (int key in damageIndex.Keys)
        {
            damageIndex[key].Clear();
            result++;
        }
        return result;
    }

    /// <summary>
    /// 2017-8-10
    /// Removes all on-going damage for the target.
    /// Returns true when successful.
    /// Returns false when the target had no on-going damage to clear.
    /// </summary>
    public static bool ClearDamageForTarget(GameObject target)
    {
        int hash = target.GetHashCode();
        if (damageIndex.ContainsKey(hash))
        {
            damageIndex[hash].Clear();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 2017-8-9
    /// Apply recurring auto-expiring damage to a target.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hitDamage"></param>
    /// <param name="ratePerSecond"></param>
    /// <param name="duration"></param>
    /// <param name="damageTypeId"></param>
    public static void AddDamageToTarget(GameObject target, float hitDamage, float ratePerSecond, float duration, int damageTypeId)
    {
        PeriodicDamage periodicDamage = new PeriodicDamage(hitDamage, ratePerSecond, duration, damageTypeId);
        DamageList damageList = GetListForTarget(target);
        damageList.Add(periodicDamage);
    }

    /// <summary>
    /// 2017-8-9
    /// Apply recurring (non-expiring) damage that is based on the target's presence in
    /// (or absence from) an area. This message begins applying damage to the target.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hitDamage"></param>
    /// <param name="ratePerSecond"></param>
    /// <param name="damageTypeId"></param>
    public static void ZoneDamageStart(GameObject target, float hitDamage, float ratePerSecond, int damageTypeId)
    {
        //  TODO
    }

    /// <summary>
    /// 2017-8-9
    /// Removes recurring (non-expiring) damage that is based on the target's presence in
    /// (or absence from) an area. This message terminates the application of damage to the target.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hitDamage"></param>
    /// <param name="ratePerSecond"></param>
    /// <param name="damageTypeId"></param>
    public static void ZoneDamageEnd(GameObject target, float hitDamage, float ratePerSecond, int damageTypeId)
    {
        //  TODO
    }

    #endregion
    #region Private Helpers

    /// <summary>
    /// 2017-8-10
    /// Returns the DamageList object associated with the target parameter.
    /// If there is no list associated, it creates one.
    /// </summary>
    /// <param name="target">The target object</param>
    private static DamageList GetListForTarget(GameObject target)
    {
        int hash = target.GetHashCode();
        DamageList result = null;
        if (damageIndex.ContainsKey(hash)) result = damageIndex[hash];
        else
        {
            result = new DamageList();
            damageIndex.Add(hash, result);
        }
        return result;
    }

    #endregion
    #region Data Structures

    private class DiscreteDamage
    {
        public float hitDamage;
        public int damageTypeId;

        //  constructors
        public DiscreteDamage() { }
        public DiscreteDamage(float hitDamage, int damageTypeId)
        {
            this.hitDamage = hitDamage;
            this.damageTypeId = damageTypeId;
        }
    }

    private class PeriodicDamage
    {
        public float damagePerHit;
        public float hitsPerSecond;
        public float duration;
        public int damageTypeId;
        public float timestamp;

        //  constructors
        public PeriodicDamage() { }
        public PeriodicDamage(float damagePerHit, float hitsPerSecond, float duration, int damageTypeId)
        {
            this.damagePerHit = damagePerHit;
            this.hitsPerSecond = hitsPerSecond;
            this.duration = duration;
            this.damageTypeId = damageTypeId;
        }

        //  converters
        public DiscreteDamage ToDiscreteDamage()
        {
            return new DiscreteDamage(damagePerHit, damageTypeId);
        }
    }
    
    /// <summary>
    /// 2017-8-9
    /// Manages a list of periodic damage objects.
    /// </summary>
    private class DamageList
    {
        #region Data

        /// <summary>
        /// 2017-8-9
        /// When a PeriodicDamage object is added to this structure, its duration value
        /// is overwritten with its expiration time.
        /// </summary>
        private List<PeriodicDamage> damageList;
        private float nextUpdate;

        #endregion
        #region Constructors

        /// <summary>
        /// 2017-8-10
        /// Constructor
        /// </summary>
        public DamageList()
        {
            damageList = new List<PeriodicDamage>();
        }

        #endregion
        #region Public API

        /// <summary>
        /// 2017-8-10
        /// Remove all PeriodicDamage objects from the list.
        /// </summary>
        public void Clear()
        {
            damageList.Clear();
        }

        /// <summary>
        /// 2017-8-9
        /// Adds the damage object to the internal damage list.
        /// </summary>
        public void Add(PeriodicDamage periodicDamage)
        {
            SetTimeStamp(periodicDamage);
            damageList.Add(periodicDamage);
        }

        /// <summary>
        /// 2017-8-9
        /// Checks the damage list and removes any damage that has exceeded it's duration.
        /// This should not be called every game Update.
        /// </summary>
        public void IrregularUpdate()
        {
            //  order the damage list by expiration time
            damageList.Sort((left, right) =>
            {
                return left.duration.CompareTo(right.duration);
            });

            //  remove any damage objects that have expired
            while (GetExpireTime(damageList[0])< Time.time) damageList.RemoveAt(0);

            //  Store the next update time
            this.nextUpdate = GetExpireTime(damageList[0]);
        }

        public DiscreteDamage[] GetCurrentDamage(float currentTime)
        {
            DiscreteDamage[] damageArray = null;
            
            //  TODO

            return damageArray;
        }

        #endregion
        #region Private Helpers

        /// <summary>
        /// 2017-8-12
        /// Sets the object's timestamp to the current time.
        /// </summary>
        /// <param name="periodicDamage">A recurring damage object</param>
        private void SetTimeStamp(PeriodicDamage periodicDamage)
        {
            periodicDamage.timestamp = Time.time;
        }

        /// <summary>
        /// 2017-8-9
        /// Returns the expiration time for the PeriodicDamage object. This
        /// is calculated using the current time and the object's duration value.
        /// </summary>
        private float GetExpireTime(PeriodicDamage periodicDamage)
        {
            return Time.time + periodicDamage.timestamp;
        }

        #endregion
    }

    #endregion
}