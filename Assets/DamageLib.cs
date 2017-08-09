using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 2017-8-9
/// Library for managing recuring damage.
/// </summary>
public static class DamageLib
{
    #region Data

    /// <summary>
    /// 2017-8-9
    /// Key: Hashcode of the target GameObject
    /// Value: DamageList object that contains all the 'on-going damage' for the target.
    /// </summary>
    private Dictionary<int, DamageList> damageIndex;

    #endregion
    #region Public API

    /// <summary>
    /// 2017-8-9
    /// Removes all on-going damage for all targets. Clears all damage lists.
    /// </summary>
    public void Clear()
    {
        //  TODO
    }

    /// <summary>
    /// 2017-8-9
    /// Removes all on-going damage for the target. Clears the damage list.
    /// </summary>
    public void ClearDamageForTarget(GameObject target)
    {
        //  TODO
    }

    /// <summary>
    /// 2017-8-9
    /// Call to apply recurring auto-expiring damage to a target.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hitDamage"></param>
    /// <param name="ratePerSecond"></param>
    /// <param name="duration"></param>
    /// <param name="damageTypeId"></param>
    public void DamageTarget(GameObject target, float hitDamage, float ratePerSecond, float duration, int damageTypeId)
    {
        //  TODO
    }

    /// <summary>
    /// 2017-8-9
    /// Call to apply recurring damage that is based on the target's presence
    /// (or absence from) an area. This message begins applying damage to the target.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hitDamage"></param>
    /// <param name="ratePerSecond"></param>
    /// <param name="damageTypeId"></param>
    public void ZoneDamageStart(GameObject target, float hitDamage, float ratePerSecond, int damageTypeId)
    {
        //  TODO
    }

    /// <summary>
    /// 2017-8-9
    /// Call to apply recurring damage that is based on the target's presence
    /// (or absence from) an area. This message ends the application of damage to the target.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hitDamage"></param>
    /// <param name="ratePerSecond"></param>
    /// <param name="damageTypeId"></param>
    public void ZoneDamageEnd(GameObject target, float hitDamage, float ratePerSecond, int damageTypeId)
    {
        //  TODO
    }

    #endregion
    #region Private Helpers
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

    #endregion
    #region Data Structure Manager

    /// <summary>
    /// 2017-8-9
    /// Manages a list of periodic damage objects.
    /// </summary>
    private class DamageList
    {
        /// <summary>
        /// 2017-8-9
        /// When a PeriodicDamage object is added to this structure, its duration value
        /// is overwritten with its expiration time.
        /// </summary>
        private List<PeriodicDamage> damageList;

        #region Public API

        /// <summary>
        /// 2017-8-9
        /// Adds the damage object to the internal damage list.
        /// The damage object's 'duration' value is converted to an
        /// expiration time-stamp.
        /// </summary>
        public void Add(PeriodicDamage periodicDamage)
        {
            periodicDamage.duration = GetExpireTime(periodicDamage);
            damageList.Add(periodicDamage);
        }

        /// <summary>
        /// 2017-8-9
        /// Checks the damage list and removes any damage that has exceeded it's duration.
        /// </summary>
        public void Update()
        {
            //  order the damage list by expiration time
            damageList.Sort((left, right) =>
            {
                return left.duration.CompareTo(right.duration);
            });

            //  remove any damage objects that have expired
            while (damageList[0].duration < Time.time) damageList.RemoveAt(0);
        }

        #endregion
        #region Private Helpers

        /// <summary>
        /// 2017-8-9
        /// Returns the expiration time for the PeriodicDamage object. This
        /// is calculated using the current time and the object's duration value.
        /// </summary>
        private float GetExpireTime(PeriodicDamage periodicDamage)
        {
            return Time.time + periodicDamage.duration;
        }

        #endregion
    }

    #endregion
}