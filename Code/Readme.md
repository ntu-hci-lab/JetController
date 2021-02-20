# How to use CSharp API

## Initial Step

You need to write your mapping functions between PWM and Force.

The relationship depends on many factors, so you have to measure the curve yourself.

``
private static int Force_To_PWM(double Force)
{
    if (Force < Force_Ignore_Threshold)
        return 0;
    else if (double.IsNaN(Force) || double.IsInfinity(Force))
        Force = One_Direction_Max_Force;

    // Please write your Force-PWM mapping function here.
    throw new NotImplementedException("Please write your Force-PWM mapping function here!");
}

private static double PWM_To_Force(int PWM)
{
    if (PWM > Max_PWM_Value)
        return One_Direction_Max_Force;  // Handle the force exceeds the maximum force of the system
    else if (PWM <= 0)
        return 0;   //Handle the unexcepted value of PWM

    // Please write your Force-PWM mapping function here.
    throw new NotImplementedException("Please write your Force-PWM mapping function here!");
}
``


## Create an object JetController
``
JetController jc = new JetController(“COM5”, 500000);
``

## When you need to predict the force magnitude
``
double[] PredictedMagnitude;
jc.SendPredictForce(2, 0, 0, ref PredictedMagnitude);
``

## Create Force Feedback
``
double[] ProducedMagnitude;
jc.ApplyForce(2, 0, 0, ref ProducedMagnitude);
``

## If you need to open/close separately
``
jc.SendValveOnOff(1, false);
``
## Close all valves at once
``
jc.CloseAllValve();
``