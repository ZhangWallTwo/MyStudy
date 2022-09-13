using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Linq;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarFitness : IFitness
    {
        public CarFitness()
        {
            ChromosomesToBeginEvaluation = new BlockingCollection<CarChromosome>();
            ChromosomesToEndEvaluation = new BlockingCollection<CarChromosome>();
        }

        public BlockingCollection<CarChromosome> ChromosomesToBeginEvaluation { get; private set; }
        public BlockingCollection<CarChromosome> ChromosomesToEndEvaluation { get; private set; }
        public double Evaluate(IChromosome chromosome)
        {
            var c = chromosome as CarChromosome;
            ChromosomesToBeginEvaluation.Add(c);//!!!16��

            do
            {
                Thread.Sleep(1000);//Sleep()	:������ �ð� ���� ���� �����带 �Ͻ� �ߴ��մϴ�.? 1000ms(1��) ���� ���
                //�̻��̿� �� ������ �̵���
                c.Fitness = c.MaxDistance + c.MaxVelocity;//!!! Fitness = �ִ�Ÿ�+�ִ�ӵ�
            } while (!c.Evaluated);//Evaluate:��

            ChromosomesToEndEvaluation.Add(c);

            do
            {
                Thread.Sleep(100);
            } while (!c.Evaluated);

            return c.MaxDistance + c.MaxVelocity;
        }

    }
}