# DevicePlugins

These plugin scripts are for sensor connection.

Each class name is important to load plugin in SDF.

For example, if it describes a name with 'RobotControl' in `<plugin>` attributesm, SDF Parser will start to find a plugin named 'RobotControl' in Unity project.

Unlink gazebo, 'filename' attribute shall not be used anywhere on current project.

```
<model>
  ...
  ...
  <plugin name='RobotControl' filename=''>
    <PID>
      <kp>3.0</kp>
      <ki>0.2</ki>
      <kd>0.0</kd>
    </PID>
    <wheel>
      <base>449</base>
      <radius>95.5</radius>
      <location type="left">LeftWheel</location>
      <location type="right">RightWheel</location>
      <friction>
        <motor>0.06</motor>
        <brake>13.0</brake>
      </friction>
    </wheel>
    <update_rate>20</update_rate>
  </plugin>
</model>
```