using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Calibration1.CalibrationTransformation;
using MathNet.Numerics.LinearAlgebra;

namespace Assets.Scripts.Body_Data.CalibrationData
{
    public class CalData
    {
        public Dictionary<string, List<Quaternion>> SensT1PoseQuat = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> SensT2PoseQuat = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> SensSPoseQuat  = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> SensZPoseQuat  = new Dictionary<string, List<Quaternion>>();

        public Dictionary<string, List<Quaternion>> transT1toZPoseQuat = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> transT2toSPoseQuat = new Dictionary<string, List<Quaternion>>();

        public Dictionary<string, List<Quaternion>> transT1toZIdealPoseQuat = new Dictionary<string, List<Quaternion>>();
        public Dictionary<string, List<Quaternion>> transT2toSIdealPoseQuat = new Dictionary<string, List<Quaternion>>();

        public Dictionary<string, List<float>> TimePose = new Dictionary<string, List<float>>();
        public Dictionary<string, List<Quaternion>> Q = new Dictionary<string, List<Quaternion>>();
        public List<float> TimeQ = new List<float>();
        public Dictionary<string, List<float>> sT1toZ = new Dictionary<string, List<float>>();
        public Dictionary<string, List<float>> sT2toS = new Dictionary<string, List<float>>();
        
        public bool T1pOn = false;
        public bool T2pOn = false;
        public bool ZpOn  = false;
        public bool SpOn  = false;
        public bool RecOn = true;
        public bool CalibrationOn = false;       

        public bool T1pEnd = false;
        public bool T2pEnd = false;
        public bool ZpEnd  = false;
        public bool SpEnd  = false;
        public bool CalibrationEnd  = false;

        public float K  = 2.0F * Mathf.PI / 360;
        public int NFrameOnM    = 10;
        public int CountFrame   = 0;
        public int CountQuarRec = 0;

        public CalData()
        {
            Q.Add("Up", new List<Quaternion>());
            Q.Add("Lo", new List<Quaternion>());

            sT1toZ.Add("Up", new List<float>());
            sT1toZ.Add("Lo", new List<float>());
            sT2toS.Add("Up", new List<float>());
            sT2toS.Add("Lo", new List<float>());

            SensT1PoseQuat.Add("Up", new List<Quaternion>());
            SensT1PoseQuat.Add("Lo", new List<Quaternion>());
            SensT2PoseQuat.Add("Up", new List<Quaternion>());
            SensT2PoseQuat.Add("Lo", new List<Quaternion>());
            TimePose.Add("T1", new List<float>());
            TimePose.Add("T2", new List<float>());

            SensSPoseQuat.Add("Up", new List<Quaternion>());
            SensSPoseQuat.Add("Lo", new List<Quaternion>());
            TimePose.Add("S", new List<float>());

            SensZPoseQuat.Add("Up", new List<Quaternion>());
            SensZPoseQuat.Add("Lo", new List<Quaternion>());
            TimePose.Add("Z", new List<float>());

            transT1toZPoseQuat.Add("Up", new List<Quaternion>());
            transT1toZPoseQuat.Add("Lo", new List<Quaternion>());
            transT1toZIdealPoseQuat.Add("Up", new List<Quaternion>());
            transT1toZIdealPoseQuat.Add("Lo", new List<Quaternion>());

            transT2toSPoseQuat.Add("Up", new List<Quaternion>());
            transT2toSPoseQuat.Add("Lo", new List<Quaternion>());           
            transT2toSIdealPoseQuat.Add("Up", new List<Quaternion>());
            transT2toSIdealPoseQuat.Add("Lo", new List<Quaternion>());
        }
        public void Association()
        {
            Debug.Log("Asso start");
            float startT1 = TimePose["T1"][0];
            float endT1   = TimePose["T1"][TimePose["T1"].Count - 1];
            float startT2 = TimePose["T2"][0];
            float endT2   = TimePose["T2"][TimePose["T2"].Count - 1];

            float tT1half = (endT1 + startT1) / 2.0F;            
            float tT2half = (endT2 + startT2) / 2.0F;

            float startZ = TimePose["Z"][0];
            float endZ   = TimePose["Z"][TimePose["z"].Count - 1];
            float tZhalf = (endZ + startZ) / 2.0F;

            float startS = TimePose["S"][0];
            float endS   = TimePose["S"][TimePose["S"].Count - 1];
            float tShalf = (endS + startS) / 2.0F;
            int nq = Q["Up"].Count;
            for (int i = 0; i < nq; i++)
            {
                if (tT1half < TimeQ[i] && TimeQ[i] < tZhalf)
                {
                    transT1toZPoseQuat["Up"].Add(Q["Up"][i]);
                    transT1toZPoseQuat["Lo"].Add(Q["Lo"][i]);
                }
                if (tT2half < TimeQ[i] && TimeQ[i] < tShalf)
                {
                    transT2toSPoseQuat["Up"].Add(Q["Up"][i]);
                    transT2toSPoseQuat["Lo"].Add(Q["Lo"][i]);
                }
            }
            Debug.Log("first loop done ");
            nq = transT1toZPoseQuat["Up"].Count;
            float kup = 0.0F;
            float klo = 0.0F; 
            for (int i = 0; i < nq; i++)
            {
                kup = Parametrisation(transT1toZPoseQuat["Up"][i], transT1toZPoseQuat["Up"][0], transT1toZPoseQuat["Up"][nq - 1]);
                klo = Parametrisation(transT1toZPoseQuat["Lo"][i], transT1toZPoseQuat["Lo"][0], transT1toZPoseQuat["Lo"][nq - 1]);
                //kup = (0.3F <= kup && Math.Round(kup) <= 1.0F) ? 0.0F : kup;
                //klo = (0.3F <= klo && Math.Round(klo) <= 1.0F) ? 0.0F : klo;

                sT1toZ["Up"].Add(kup);
                sT1toZ["Lo"].Add(klo);
                Vector3 vT1toZ = new Vector3(0.0F, -90.0F * sT1toZ["Up"][i], 0.0F);
                transT1toZIdealPoseQuat["Up"].Add(Quaternion.Euler(vT1toZ));
                vT1toZ = new Vector3(0.0F, -90.0F * sT1toZ["Lo"][i], 0.0F);
                transT1toZIdealPoseQuat["Lo"].Add(Quaternion.Euler(vT1toZ));
            }
            Debug.Log("second loop done ");
            nq = transT2toSPoseQuat["Up"].Count;
            for (int i = 0; i < nq; i++)
            {
                kup = Parametrisation(transT2toSPoseQuat["Up"][i], transT2toSPoseQuat["Up"][0], transT2toSPoseQuat["Up"][nq - 1]);
                klo = Parametrisation(transT2toSPoseQuat["Lo"][i], transT2toSPoseQuat["Lo"][0], transT2toSPoseQuat["Lo"][nq - 1]);                
                //kup = (0.3F <= kup && Math.Round(kup) <= 1.0F) ? 0.0F : kup;
                //klo = (0.3F <= klo && Math.Round(klo) <= 1.0F) ? 0.0F : klo;

                sT2toS["Up"].Add(kup);
                sT2toS["Lo"].Add(klo);
                Vector3 vT2toS = new Vector3(0.0F, 0.0F, -90.0F * sT2toS["Up"][i]);
                transT2toSIdealPoseQuat["Up"].Add(Quaternion.Euler(vT2toS));
                vT2toS = new Vector3(0.0F, 0.0F, -90.0F * sT2toS["Lo"][i]);
                transT2toSIdealPoseQuat["Lo"].Add(Quaternion.Euler(vT2toS));
            }
            Debug.Log("third loop done ");
            Debug.Log("End ");
        }
        public float Parametrisation(Quaternion q, Quaternion qi, Quaternion qf)
        {
            float s = (float)(Math.Sqrt((q.w  - qi.w) * (q.w  - qi.w) + (q.x  - qi.x) * (q.x  - qi.x) + (q.y  - qi.y) * (q.y  - qi.y) + (q.z  - qi.z) * (q.z  - qi.z)) /
                              Math.Sqrt((qf.w - qi.w) * (qf.w - qi.w) + (qf.x - qi.x) * (qf.x - qi.x) + (qf.y - qi.y) * (qf.y - qi.y) + (qf.z - qi.z) * (qf.z - qi.z)));
            return s;
        }
        public void UpdateStates(float CurrentT)
        {           
            float tStartT1 = 0.0F;
            float tEndT1   = 12.0F;

            //float tStartTZ = 12.0F;
            //float tEndTZ = 15.0F;

            float tStartZ = 15.0F;
            float tEndZ   = 17.0F;

            //float tStartZT = 17.0F;
            //float tEndZT = 22.0F;

            float tStartT2 = 22.0F;
            float tEndT2   = 24.0F;

            //float tStartTS = 23.0F;
            //float tEndTS = 24.0F;

            float tStartS = 24.0F;
            float tEndS   = 29.0F;

            T1pOn  = (tStartT1 < CurrentT && CurrentT < tEndT1);
            T1pEnd = (tEndT1   < CurrentT                     );

            T2pOn  = (tStartT2 < CurrentT && CurrentT < tEndT2);
            T2pEnd = (tEndT2   < CurrentT                     );

            ZpOn   = (tStartZ  < CurrentT && CurrentT < tEndZ );
            ZpEnd  = (tEndZ    < CurrentT                     );

            SpOn   = (tStartS  < CurrentT && CurrentT < tEndS );
            SpEnd  = (tEndS    < CurrentT                     );
                                   
            CalibrationOn = (SpEnd && !CalibrationEnd);
        }
        public void Calibration()
        {            
            Association();
            ShiuTransform s = new ShiuTransform();
            int iZphalf = (int)Math.Round(TimePose["Z"].Count / 2.0F);
            int iSphalf = (int)Math.Round(TimePose["S"].Count / 2.0F);            
            Matrix4x4 Bz = Matrix4x4.TRS(Vector3.zero, SensZPoseQuat["Up"][iZphalf], Vector3.one);
            Matrix4x4 Bs = Matrix4x4.TRS(Vector3.zero, SensSPoseQuat["Up"][iSphalf], Vector3.one);           
            Matrix<float> B1 = UtoM(Bz);
            Matrix<float> B2 = UtoM(Bs);
            Matrix<float> X = s.Shiufunc(B1, "Z", B2, "S");
            ParkTransform p = new ParkTransform();
            Vector<float> v = p.PoseToEulerAngles("Z");
            Matrix<float> IdealAz = RotationMatrix(v[0],v[1],v[2]);
            v = p.PoseToEulerAngles("S");
            Matrix<float> IdealAs = RotationMatrix(v[0], v[1], v[2]);
            Quaternion Aq1    ;    Quaternion Aq2;
            Quaternion Bq1    ;    Quaternion Bq2;
            Matrix4x4  Am1    ;    Matrix4x4  Am2;
            Matrix4x4  Bm1    ;    Matrix4x4  Bm2;
            Matrix<float> a1  ;    Matrix<float> a2;
            Matrix<float> b1  ;    Matrix<float> b2;
            Matrix<float> M    = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> Zero = Matrix<float>.Build.Dense(3, 3, 0);
            int Np  = 4;
            int L1  = transT1toZIdealPoseQuat["Up"].Count;
            int L2  = transT2toSIdealPoseQuat["Up"].Count;
            M = p.Parkfunc(B1, IdealAz, "Add");
            M = p.Parkfunc(B2, IdealAs, "Add");
            Print(IdealAz, "Aidz", 2);
            Print(B1     , "Bz " , 2);
            Print(IdealAs, "Aids", 2);
            Print(B2     , "Bs"  , 2);
            Matrix<float> One = Matrix<float>.Build.DenseIdentity(3);
            Debug.Log("Z: ");
            Vector<float> IP1 = FindIndexNewPoses(Np, L1, sT1toZ["Up"]);
            Debug.Log("S: ");
            Vector<float> IP2 = FindIndexNewPoses(Np, L2, sT2toS["Up"]);
            for (int i = 0; i < Np; i++)
            {
                if (IP1[i] != 0)
                {
                    Aq1 = transT1toZIdealPoseQuat["Up"][(int)IP1[i]];
                    Bq1 = transT1toZPoseQuat["Up"][(int)IP1[i]];
                    Am1 = Matrix4x4.TRS(Vector3.zero, Aq1, Vector3.one);
                    Bm1 = Matrix4x4.TRS(Vector3.zero, Bq1, Vector3.one);
                    a1  = UtoM(Am1);
                    b1  = UtoM(Bm1);
                    float DOneA1 = MatrixDiff(One, a1);
                    Debug.Log("DOneA1:  " + DOneA1);
                    Print(a1, "A1in  ", 2);
                    Print(b1, "B1in  ", 2);
                    if (DOneA1 > 0.1F)
                    {
                        Debug.Log("DOneA1:  " + DOneA1);
                        Print(a1, "A1in  ", 2);
                        Print(b1, "B1in  ", 2);
                        M = p.Parkfunc(b1, a1, "Add");
                    }
                }
                if (IP2[i] != 0)
                {
                    Aq2 = transT2toSIdealPoseQuat["Up"][(int)IP2[i]];
                    Bq2 = transT2toSPoseQuat["Up"][(int)IP2[i]];
                    Am2 = Matrix4x4.TRS(Vector3.zero, Aq2, Vector3.one);
                    Bm2 = Matrix4x4.TRS(Vector3.zero, Bq2, Vector3.one);
                    a2 = UtoM(Am2);
                    b2 = UtoM(Bm2);
                    float DOneA2 = MatrixDiff(One, a2);
                    Debug.Log("DOneA2:  " + DOneA2);
                    Print(a2, "A2in  ", 2);
                    Print(b2, "B2in  ", 2);
                    if (DOneA2 > 0.1F)
                    {
                        Debug.Log("DOneA2:  " + DOneA2);
                        Print(a2, "A1in  ", 2);
                        Print(b2, "B1in  ", 2);
                        M = p.Parkfunc(b2, a2, "Add");
                    }
                }
            }
            M = p.Parkfunc(Zero, Zero, "Eig");
            /*Debug.Log("Transformations X and M");
            Print(X, "x", 2);
            Print(M, "M", 2);
            Print(p.vec , "v", 2);
            Print(p.Eigvec, "Eigvec", 2);
            Print(p.vec , "v", 2);
            Debug.Log("Eigval  : "+ p.Eigval);
            Debug.Log("End ");*/
            /////////////////////////////////
            /////////////////////////////////
            CalibrationEnd = true;///////////
            /////////////////////////////////
            /////////////////////////////////
        }
        public Vector<float> FindIndexNewPoses(int Np, int Le, List<float> List)
        {            
            float si ;
            int i = 0;
            int k = 1;
            Vector<float> ListIndexNewPoses = Vector<float>.Build.Dense(Np, 0);            
            while(i < Le && k <= Np)
            {
                si = List[i];
                Debug.Log(i + "    k / (Np + 1.0):  " + k / (Np + 1.0) + "   Si:  " + si);
                if ( ((si + 0.04F) > k / (Np + 1.0) && (si - 0.04F) < k / (Np + 1.0)) && k<=Np )
                {
                    ListIndexNewPoses[k-1] = i;
                    k++;
                }
                i++;
            }
            return ListIndexNewPoses;
        }
        public static Matrix<float> RotationMatrix(float tx, float ty, float tz)
        {
            Matrix<float> Rx = Matrix<float>.Build.Dense(3,3,0);
            Matrix<float> Ry = Matrix<float>.Build.Dense(3,3,0);
            Matrix<float> Rz = Matrix<float>.Build.Dense(3,3,0);
            Rx[0, 0] =  1.0F                ; Rx[0, 1] = 0.0F                ; Rx[0, 2] = 0.0F                 ;
            Rx[1, 0] =  0.0F                ; Rx[1, 1] = 1.0F * Mathf.Cos(tx); Rx[1, 2] =-1.0F * Mathf.Sin(tx) ;
            Rx[2, 0] =  0.0F                ; Rx[2, 1] = 1.0F * Mathf.Sin(tx); Rx[2, 2] = 1.0F * Mathf.Cos(tx) ;

            Ry[0, 0] =  1.0F * Mathf.Cos(ty); Ry[0, 1] = 0.0F                ; Ry[0, 2] =  1.0F * Mathf.Sin(ty);
            Ry[1, 0] =  0.0F                ; Ry[1, 1] = 1.0F                ; Ry[1, 2] =  0.0F                ;
            Ry[2, 0] = -1.0F * Mathf.Sin(ty); Ry[2, 1] = 0.0F                ; Ry[2, 2] =  1.0F * Mathf.Cos(ty);

            Rz[0, 0] =  1.0F * Mathf.Cos(tz); Rz[0, 1] =-1.0F * Mathf.Sin(tz); Rz[0, 2] = 0.0F                 ;
            Rz[1, 0] =  1.0F * Mathf.Sin(tz); Rz[1, 1] = 1.0F * Mathf.Cos(tz); Rz[1, 2] = 0.0F                 ;
            Rz[2, 0] =  0.0F                ; Rz[2, 1] = 0.0F                ; Rz[2, 2] = 1.0F                 ;           
            return Ry*Rx*Rz;
        }
        public void Print(Matrix<float> M, string Name, int n)
        {
            
            Debug.Log(Name + ":");
            Debug.Log("    |" + Math.Round(M[0, 0], n) + "," + Math.Round(M[0, 1], n) + "," + Math.Round(M[0, 2], n) + "|" + '\n'
                    + "    |" + Math.Round(M[1, 0], n) + "," + Math.Round(M[1, 1], n) + "," + Math.Round(M[1, 2], n) + "|" + '\n');
            Debug.Log("    |" + Math.Round(M[2, 0], n) + "," + Math.Round(M[2, 1], n) + "," + Math.Round(M[2, 2], n) + "|" );
        }
        public float UMatrixDiff(Matrix4x4 A, Matrix4x4 B)
        {
            Matrix<float> D = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> a = Matrix<float>.Build.Dense(3, 3, 0);
            float NA ;
            float ND ;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    D[i, j] = A[i, j] - B[i, j];
                    a[i, j] = A[i, j];
                }
            }
            NA = MatrixNorm(a);
            ND = MatrixNorm(D);
            return ND / NA;
        }
        public float MatrixDiff(Matrix<float> A, Matrix<float> B)
        {
            Matrix<float> D = Matrix<float>.Build.Dense(3, 3, 0);
            D = B - A;
            float NA = MatrixNorm(A);
            float ND = MatrixNorm(D);
            return ND/NA;
        }
        public float MatrixNorm(Matrix<float> A)
        {
            float NA = 0.0F;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NA = NA + A[i, j] * A[i, j];
                }
            }
            NA = Mathf.Sqrt(NA);
            return NA;
        }
        public Matrix<float> UtoM(Matrix4x4 U)
        {
            Matrix<float> M = Matrix<float>.Build.Dense(3, 3);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    M[i, j] = U[i, j];
                }
            }
            return M;
        }
        public Matrix4x4 MtoU(Matrix<float> M)
        {
            Matrix4x4 U = Matrix4x4.zero;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    U[i, j] = M[i, j];
                }
            }
            U[3, 3] = 1F;
            return U;
        }
    }
}





