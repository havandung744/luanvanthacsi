﻿using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Excel.ClassExcel
{
    public class EvaluationBoardExcel
    {
        public virtual int stt { get; set; }
        public virtual string StudentName { get; set; }
        public virtual string TopicName { get; set; }
        public virtual string Branch { get; set; }
        public virtual string InstructorOne { get; set; }
        public virtual string OnstructorTwo { get; set; }
        public virtual string PresidentName { get; set; }
        public virtual string CounterattackerOne { get; set; }
        public virtual string CounterattackerTwo { get; set; }
        public virtual string CounterattackerThree { get; set; }
        public virtual string ScientistOne { get; set; }
        public virtual string ScientistTwo { get; set; }
        public virtual string SecretaryName { get; set; }
    }
}
