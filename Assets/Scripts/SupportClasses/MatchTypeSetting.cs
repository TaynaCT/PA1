﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.SupportClasses
{
    public class MatchTypeSetting : Singleton<MatchTypeSetting>
    {
        //private static MatchTypeSetting _instance;
        //public static MatchTypeSetting Instance()
        //{
        //    if (_instance != null)
        //    {
        //        return _instance;
        //    }
        //    Debug.Log("Is Null!!");
        //    return null;
        //}

        //private void Awake()
        //{
        //    _instance = this;
        //}
        
        public MatchType Type { get; set; }
        private void Awake()
        {
            Type = MatchType.None;
        }
    }
}