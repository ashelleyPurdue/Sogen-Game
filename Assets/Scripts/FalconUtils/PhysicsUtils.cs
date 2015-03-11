using UnityEngine;
using System.Collections;

public static class PhysicsUtils
{
    //--Aim projectile--
    
    public static Vector2 AimProjectileTime2D(Vector2 start, Vector2 end, float time, float gravity)
    {
        //Returns the velocity vector a projectile should be launched at to hit a target in 2D space, given a set travel time.
        
        Vector2 output = Vector2.zero;
        
        output.x = (end.x - start.x) / time;
        output.y = ((end.y - start.y) / time) - (gravity * time / 2);
        
        return output;
    }
    
    public static Vector2 AimProjectile2D(Vector2 start, Vector2 end, float velocity, float gravity, float tolerance = 0.1f)
    {
        //Returns the velocity vector a projectile should be launched at to hit a target in 2D space, given a set initial velocity.
        
        //Find the launch angle.
        ProjectileSimulator2D simulator = new ProjectileSimulator2D(start, end, velocity, gravity, tolerance);
        
        float increment = 360 * Mathf.Deg2Rad / 1000;
        
        float angle = Utils.GuessValue(0, simulator.WouldHit, increment, false);
        
        //Return the velocity vecotr
        Vector2 output = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        output *= velocity;
        
        return output;
    }
    
    public class ProjectileSimulator2D
    {
        public readonly Vector2 start;
        public readonly Vector2 end;
        
        public readonly float velocity;
        public readonly float gravity;
        public readonly float tolerance;
        
        public ProjectileSimulator2D(Vector2 start, Vector2 end, float velocity, float gravity, float tolerance)
        {
            this.start = start;
            this.end = end;
            this.velocity = velocity;
            this.gravity = gravity;
            this.tolerance = tolerance;
        }
        
        public bool WouldHit(float angle)
        {
            //Returns if the projectile would hit the target, or get close enough.
            //Angle is in radians
            
            //Shift the world so that end.y = 0
            Vector2 shiftStart = start;
            Vector2 shiftEnd = end;
            
            shiftStart.y -= end.y;
            shiftEnd.y = 0;
            
            //Find the range of the shifted projectile
            float cosPart = velocity * Mathf.Cos(angle) / gravity;
            float sinPart = velocity * Mathf.Sin(angle);
            float insideSqrt = (sinPart * sinPart) + (2 * gravity * shiftStart.y);
            float sqrtPart = Mathf.Sqrt(insideSqrt);
            
            float range = cosPart * (sinPart + sqrtPart);
            
            //Find the difference between where we landed and where the end is.
            float landedX = shiftStart.x + range;
            float diff = landedX - shiftEnd.x;
            
            //Debug.Log("diff: " + diff);
            
            //Return if the difference is within the tolerance.
            return (Mathf.Abs(diff) <= tolerance);
        }
    }
}
