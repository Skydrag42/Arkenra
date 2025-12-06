using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCustomDamping : MonoBehaviour
{
    public enum CamType
	{
        FreeLook,
        Virtual
	}

    public CamType camType = CamType.FreeLook;
    private CinemachineFreeLook freeCam;
    private CinemachineVirtualCamera virtualCam;
    public Transform target;
    public float minDistance = 2f;
	public float resetDampSpeed = 10;

    private float defaultDamp;
    private CinemachineOrbitalTransposer[] freeLookTransposers = new CinemachineOrbitalTransposer[3];
    private CinemachineTransposer virtualTransposer;

    private bool changed = false;

	private void Awake()
	{
        if (camType == CamType.FreeLook)
        {
            freeCam = GetComponent<CinemachineFreeLook>();
            for (int i = 0; i < 3; ++i)
            {
                var rig = freeCam.GetRig(i);
                freeLookTransposers[i] = rig.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            }
            defaultDamp = freeLookTransposers[0].m_ZDamping;
        }
        else if (camType == CamType.Virtual)
		{
            virtualCam = GetComponent<CinemachineVirtualCamera>();
            virtualTransposer = virtualCam.GetCinemachineComponent<CinemachineTransposer>();
            defaultDamp = virtualTransposer.m_ZDamping;
		}
	}

	void Update()
    {
        float dst = Vector3.Distance(Camera.main.transform.position, target.position);
        if (dst < minDistance)
		{
            changed = true;
        }
        if (changed)
		{
            Vector3 camFwd = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
            if (dst > minDistance && Vector3.Dot(target.forward, camFwd) > 0)
            {
                changed = false;
                if (camType == CamType.FreeLook)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        freeLookTransposers[i].m_ZDamping = defaultDamp;
                    }
                }
                else if (camType == CamType.Virtual)
				{
                    virtualTransposer.m_ZDamping = defaultDamp;
				}
            }
            else
			{
                if (camType == CamType.FreeLook)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        freeLookTransposers[i].m_ZDamping = Mathf.Lerp(freeLookTransposers[i].m_ZDamping, 0f, Time.deltaTime * resetDampSpeed);
                    }
                }
                else if (camType == CamType.Virtual)
				{
                    virtualTransposer.m_ZDamping = Mathf.Lerp(virtualTransposer.m_ZDamping, 0f, Time.deltaTime * resetDampSpeed);
                }
            }
        }
    }
}
