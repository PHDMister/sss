 float3 RotateXY(float3 R,float degrees)
{
    float3 reflUVW = R;
    half theta = degrees * PI / 180.0f;
    half costha = cos(theta);
    half sintha = sin(theta);
    reflUVW = half3(reflUVW.x * costha - reflUVW.z * sintha, reflUVW.y, reflUVW.x * sintha + reflUVW.z * costha);
    return reflUVW;
}

float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
{
	original -= center;
	float C = cos( angle );
	float S = sin( angle );
	float t = 1 - C;
	float m00 = t * u.x * u.x + C;
	float m01 = t * u.x * u.y - S * u.z;
	float m02 = t * u.x * u.z + S * u.y;
	float m10 = t * u.x * u.y + S * u.z;
	float m11 = t * u.y * u.y + C;
	float m12 = t * u.y * u.z - S * u.x;
	float m20 = t * u.x * u.z - S * u.y;
	float m21 = t * u.y * u.z + S * u.x;
	float m22 = t * u.z * u.z + C;
	float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
	return mul( finalMatrix, original ) + center;
}

half3 GrassWind(float windHeight,float windIntensity,float windSpeed,float windDir,float windSizeBig,float windSizeSmall,float3 positionWS)
{

 	half3 windDir1 = half3(0,0,1);
    half3 rotateAxix = cross(windDir1,half3(0,1,0));
    half3 Speed = _Time.y * windSpeed * half3(0.5,-0.5,-0.5);
	half3 rotateXY = RotateXY(positionWS,windDir);
    //big triangleWave
    half3 bigTri= abs(frac(windDir1 * Speed + (rotateXY* rcp(windSizeBig)) + 0.5) * 2  - 1 ) ;
    half bigTriWind =dot( bigTri * bigTri * (3 - bigTri * 2),windDir1);
    //small triangleWave
    half3 smallTri = abs(frac(Speed + (rotateXY * rcp( windSizeSmall)) + 0.5) * 2 - 1);
    half smallTriWind = distance(smallTri* smallTri * ( 3 - (smallTri * 2)),half3(0,0,0));

    //rotate about axis
    half rotationAngle = (bigTriWind + smallTriWind )* PI;
    half3 pivotPoint =half3(0,0,0)- half3(0,0.1,0);
    half3 offset = (RotateAroundAxis(pivotPoint,positionWS,normalize(rotateAxix),rotationAngle) - positionWS) * windHeight *windIntensity * 0.1 ;
    return offset;
}