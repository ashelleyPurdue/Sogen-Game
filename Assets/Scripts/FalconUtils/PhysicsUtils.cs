using UnityEngine;
using System.Collections;

public static class PhysicsUtils
{
    //--Aim projectile--
    
    public static Vector2 AimProjectileTime2D(Vector2 start, Vector2 end, float time, float gravity)
    {
        //Returns the velocity vector a projectile should be launched at to hit a target in 2D space, given a set travel time.
        
        //Calculate the output
        Vector2 output = Vector2.zero;
        
        output.x = (end.x - start.x) / time;
        output.y = ((end.y - start.y) / time) - (gravity * time / 2);
        
        return output;
    }
    
    public static Vector2 AimProjectile2D(Vector2 start, Vector2 end, float velocity, float gravity, float tolerance = 0.1f)
    {
        //Returns the velocity vector a projectile should be launched at to hit a target in 2D space, given a set initial velocity.
        
        //Throw an error if it can't reach.
        float maxRange = FindProjectileRange2D(start.y - end.y,
            new Vector2(velocity * Mathf.Cos(Mathf.PI / 4), velocity * Mathf.Cos(Mathf.PI / 4)),
            gravity);
        
        if (Mathf.Abs(maxRange) < Mathf.Abs(start.x - end.x))
        {
            throw new ProjectileCantReachException();
        }
        
        //Find the launch angle.
        ProjectileSimulator2D simulator = new ProjectileSimulator2D(start, end, velocity, gravity, tolerance);
        
        float[] increments = {Mathf.PI / 4, Mathf.PI / 8, 1f};
        
        float angle = Utils.GuessValue(0, simulator.WouldHit, increments, false);
        
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
            float range = FindProjectileRange2D(shiftStart.y, new Vector2(velocity * Mathf.Cos(angle), velocity * Mathf.Sin(angle)), gravity);
            
            //Find the difference between where we landed and where the end is.
            float landedX = shiftStart.x + range;
            float diff = landedX - shiftEnd.x;
            
            //Debug.Log("diff: " + diff);
            
            //Return if the difference is within the tolerance.
            return (Mathf.Abs(diff) <= tolerance);
        }
    }

    //--Find projectile range--
    
    public static float FindProjectileRange2D(float height, Vector2 velocity, float gravity)
    {
        //Returns how far a projectile will fly before hitting the ground
        
        float cosPart = velocity.x / gravity;
        float sinPart = velocity.y;
        float insideSqrt = (sinPart * sinPart) + (2 * gravity * height);
        float sqrtPart = Mathf.Sqrt(insideSqrt);
        
        return cosPart * (sinPart + sqrtPart);
    }
}

public class ProjectileCantReachException : System.Exception
{
}
