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
            ChromosomesToBeginEvaluation.Add(c);//!!!16개

            do
            {
                Thread.Sleep(1000);//Sleep()	:지정된 시간 동안 현재 스레드를 일시 중단합니다.? 1000ms(1초) 동안 대기
                //이사이에 차 앞으로 이동함
                c.Fitness = c.MaxDistance + c.MaxVelocity;//!!! Fitness = 최대거리+최대속도
            } while (!c.Evaluated);//Evaluate:평가

            ChromosomesToEndEvaluation.Add(c);

            do
            {
                Thread.Sleep(100);
            } while (!c.Evaluated);

            return c.MaxDistance + c.MaxVelocity;
        }

    }
}