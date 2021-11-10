using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public static class Extensions
{
	public static TResult Tap<TObject, TResult>(this TObject obj, Func<TObject, TResult> func) {
		return func(obj);
	}

	public static List<T> Sorted<T>(this List<T> list) {
		List<T> result = new List<T>(list);
		result.Sort();
		return result;
	}

	public static List<T> Reversed<T>(this List<T> list) {
		List<T> result = new List<T>(list);
		result.Reverse();
		return result;
	}

	public static void Destroy(GameObject go) {
		if (Editor()) {
			UnityEngine.GameObject.DestroyImmediate(go);
		} else {
			UnityEngine.GameObject.Destroy(go);
		}
	}

	public static double sqr(this System.Object any, double x) {
        return x * x;
    }

	public static int sqr(this System.Object any, int x) {
		return x * x;
	}

    public static void swap<T>(this List<T> list, int x, int y) {
        T buf = list[x];
        list[x] = list[y];
        list[y] = buf;
    }

    public static void swap<T>(ref T x, ref T y) {
        T buf = x;
        x = y;
        y = buf;
    }

    public static T rnd<T>(this List<T> list) where T : class {
        if (list.Count == 0) return null;
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T rndExcept<T>(this List<T> list, ICollection<T> except) where T : class {
        return list.Except(except).ToList().rnd();
    }

	public static T rndExcept<T>(this List<T> list, params T[] except) where T : class {
		return list.Except(except).ToList().rnd();
	}

    public static T minBy<T>(this List<T> list, Func<T, double> criteria) where T : class {
        T result = null;
        foreach (T x in list) {
            if (result == null || criteria(result) > criteria(x)) {
                result = x;
            }
        }
        return result;
    }

    public static List<T> Shuffled<T>(this List<T> list) {
        List<T> result = new List<T>(list);
        for (int i = 0; i < result.Count; i++) {
            int x = UnityEngine.Random.Range(i, result.Count);
            T buf = result[i];
            result[i] = result[x];
            result[x] = buf;
        }
        return result;
    }

	public static List<T> ShuffledPartially<T>(this List<T> list, float swapsPerElement = 1) {
		List<T> result = new List<T>(list);
		for (int i = 0; i < swapsPerElement * result.Count; i++) {
			var j = UnityEngine.Random.Range(0, result.Count - 1);
			result.swap(j, j+1);
		}
		return result;
	}

    public static List<T> Shuffled<T>(this List<T> list, int from = 0, int to = -1) {
        to = modulo(to, list.Count);
        List<T> result = new List<T>(list);
        for (int i = from; i <= to; i++) {
            int x = UnityEngine.Random.Range(i, to + 1);
            T buf = result[i];
            result[i] = result[x];
            result[x] = buf;
        }
        return result;
    }

    public static int modulo(this int x, int y) {
        return (x % y + y) % y;
    }

    public static T Cyclic<T>(this List<T> list, int index = 1) {
        return list[modulo(index, list.Count)];
    }

    public static T CyclicNext<T>(this List<T> list, T element, int delta = 1) {
        if (list.IndexOf(element) == -1) {
            return default(T);
        }
        return list.Cyclic(list.IndexOf(element) + delta);
    }

    public static string Path(this GameObject obj) {
        string path = "/" + obj.name;
        while (obj.transform.parent != null) {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    public static Vector2 xy(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    public static int ExtMin<T>(this IEnumerable<T> collection, Func<T, int> criteria) {
        if (collection.Count() == 0) {
            return int.MaxValue;
        }
        return collection.Min(criteria);
    }

    public static int ExtMax<T>(this IEnumerable<T> collection, Func<T, int> criteria) {
        if (collection.Count() == 0) {
            return int.MinValue;
        }
        return collection.Max(criteria);
    }

    public static float ExtMin<T>(this IEnumerable<T> collection, Func<T, float> criteria) {
        if (collection.Count() == 0) {
            return float.PositiveInfinity;
        }
        return collection.Min(criteria);
    }

    public static T MinBy<T>(this IEnumerable<T> collection, Func<T, float> criteria) {
        float best = float.PositiveInfinity;
        T result = default(T);
        foreach(T el in collection) {
            float cand = criteria(el);
            if (cand < best) {
                result = el;
                best = cand;
            }
        }
        return result;
    }

	public static T MaxBy<T>(this IEnumerable<T> collection, Func<T, float> criteria) {
		return collection.MinBy(x => -criteria(x));
	}	

    public static float ExtMax<T>(this IEnumerable<T> collection, Func<T, float> criteria) {
        if (collection.Count() == 0) {
            return float.NegativeInfinity;
        }
        return collection.Max(criteria);
    }

    public static string ExtToString<T>(this IEnumerable<T> collection, string delimiter = ", ", string format = "[{0}]", Func<T, string> elementToString = null) {
        elementToString = elementToString ?? (obj => obj.ToString());
        return String.Format(format, String.Join(delimiter, collection.Select(obj => obj != null ? elementToString(obj) : "null").ToArray()));
    }

    public static void ChangeAlpha(this Material material, float alpha) {
        Color c = material.color;
        c.a = alpha;
        material.color = c;
    }

    public static List<T> GetComponentsInDirectChildren<T>(this Component component) {
        List<T> result = new List<T>();
        foreach (Transform t in component.transform) {
            result = result.Concat(t.GetComponents<T>().ToList()).ToList();
        }
        return result;
    }

    public static Vector3 WithZ(this Vector2 v, float z) {
        return new Vector3(
            v.x, 
            v.y,
            z
        );
    }

    public static Vector3 Change(this Vector3 v, float x = float.NaN, float y = float.NaN, float z = float.NaN) {
        return new Vector3(
            float.IsNaN(x) ? v.x : x,
            float.IsNaN(y) ? v.y : y,
            float.IsNaN(z) ? v.z : z
        );
    }
    public static Color Change(this Color c, float a = float.NaN, float r = float.NaN, float g = float.NaN, float b = float.NaN) {
        return new Color(
            float.IsNaN(r) ? c.r : r,
            float.IsNaN(g) ? c.g : g,
            float.IsNaN(b) ? c.b : b,
            float.IsNaN(a) ? c.a : a
        );
    }

    public static Vector3 NormalizeAngles(Vector3 angles) {
        return new Vector3(NormalizeAngle(angles.x), NormalizeAngle(angles.y), NormalizeAngle(angles.z));
    }

    public static string ExtToString(this Vector2 v) {
        return String.Format("({0:0.####}, {1:0.####})", v.x, v.y);
    }

    public static List<T> ShallowClone<T>(this List<T> listToClone) {
        if (listToClone == null) {
            return null;
        }
        return listToClone.Select(item => item).ToList();
    }

    public static string Path(this Transform transform) {
        string path = transform.name;
        while (transform.parent != null) {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }

    public static void Reset(this Transform transform) {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    public static T Rand<T>(this T[,] matrix) {
        return matrix[UnityEngine.Random.Range(0, matrix.GetLength(0)), UnityEngine.Random.Range(0, matrix.GetLength(1))];
    }

    public static string ExtToString(this Transform t) {
        return String.Format("{{position = {0}, rotation = {1}, lossyScale = {2}}}", t.position.ExtToString(), t.rotation, t.lossyScale.ExtToString());
    }

    public static string ExtToString(this Ray ray) {
        return String.Format("{{origin = {0}, direction = {1}}}", ray.origin.ExtToString(), ray.direction.ExtToString());
    }

    static Matrix4x4 x;
    static Matrix4x4 y;


    static bool CloseMatrix(Transform a, Transform b) {
        x = a.localToWorldMatrix;
        y = b.localToWorldMatrix;
        for (int i = 0; i < 16; i++) {
            if (Mathf.Abs(x[i] - y[i]) > 0.01f) {
                return false;
            }
        }
        return true;
        //return a.localToWorldMatrix == b.localToWorldMatrix;
    }

    public static void IgnoreCollision(Component a, Component b, bool ignore = true) {
        a.GetComponents<Collider>().ForEach(c1 => {
            b.GetComponents<Collider>().ForEach(c2 => {
                IgnoreCollision(c1, c2, ignore);
            });
        });
    }

    public static List<T> GetComponentsInMyChildren<T>(this MonoBehaviour mb) where T : MonoBehaviour {
        List<T> res = new List<T>();
        foreach (Transform c in mb.transform) {
            T component = c.GetComponent<T>();
            if (component != null) {
                res.Add(component);
            }
        }
        return res;
    }

    public static List<Transform> Children(this Transform t) {
        List<Transform> res = new List<Transform>();
        foreach (Transform c in t) {
            res.Add(c);
        }
        return res;
    }

    public static int Modulo(int x, int y) {
        return (x % y + y) % y;
    }

    public static int Int(this UnityEngine.UI.Slider slider) {
        return (int)(slider.value);
    }

    public static float Direction(this Vector2 vector) {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

	public static T Rnd<T>(this IEnumerable<T> collection) {
		int cnt = collection.Count();
		if (cnt == 0) {
			return default(T);
		}
		return collection.ElementAt(UnityEngine.Random.Range(0, cnt));
	}

    public static T Rnd<T>(this List<T> collection) {
        return collection[UnityEngine.Random.Range(0, collection.Count)];
    }

    public static T Rnd<T>(this List<T> collection, Func<T, float> weight) {
        float totalWeight = collection.Sum(weight);
        float rndValue = UnityEngine.Random.Range(0, totalWeight);
        float skipped = 0;
        for (int i = 0; i < collection.Count; i++) {
            skipped += weight(collection[i]);
            if (skipped > rndValue) {
                return collection[i];
            }
        }
        Debug.LogFormat("skipped: {0}", skipped);
        Debug.LogFormat("totalWeight: {0}", totalWeight);
        throw new Exception("Wrong weighted rnd function!");
    }

    public static void TryPlay(this MonoBehaviour go, AudioSource audioSource) {
        if (audioSource != null) {
            audioSource.Play();
        }
    }

	public static Vector3 withZ(this Vector2 v, float z) {
		return new Vector3(v.x, v.y, z);
	}

    public static Vector4 Timed(this Vector3 v, float t) {
        return new Vector4(v.x, v.y, v.z, t);
    }

    public static Vector3 xyz(this Vector4 v) {
        return new Vector3(v.x, v.y, v.z);
    }

    public static float NormalizeAngle(float angle) {
        while (angle < -180) {
            angle += 360;
        }
        while (angle > 180) {
            angle -= 360;
        }
        return angle;
    }

    public static string ExtToString(this Vector3 v) {
        return String.Format("({0:0.####}, {1:0.####}, {2:0.####})", v.x, v.y, v.z);
    }
    
    static bool CloseFourVectors(Transform a, Transform b) {
        return ClosePosition(a, b) && CloseUp(a, b) && CloseForward(a, b) && CloseScale(a, b);
    }

    public static bool Close(Transform a, Transform b) {
        return CloseFourVectors(a, b);
    }

    public static bool ClosePosition(Transform a, Transform b) {
        return Close(a.position, b.position);
    }

    public static bool CloseUp(Transform a, Transform b) {
        return Close(a.up, b.up);
    }

    public static bool CloseForward(Transform a, Transform b) {
        return Close(a.forward, b.forward);
    }

    public static bool CloseScale(Transform a, Transform b) {
        return Close(a.lossyScale, b.lossyScale);
    }

    public static bool Close(Vector3 a, Vector3 b) {
        return Vector3.SqrMagnitude(a - b) < 0.01f;
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        foreach (T element in source) {
            action(element);
        }
    }

    public static List<T> RndSelection<T>(this List<T> collection, int cnt) {
        if (cnt > collection.Count) {
            return collection;
        }
        List<T> result = new List<T>();
        int trash = collection.Count - cnt;
        collection.ForEach(x => {
            if (UnityEngine.Random.Range(0, cnt + trash) < cnt) {
                result.Add(x);
                --cnt;
            } else {
                --trash;
            }
        });
        return result;
    }

    public static List<T> Range<T>(this List<T> collection, int from, int to) {
        if (from < 0) {
            from = 0;
        }
        if (to >= collection.Count) {
            to = collection.Count - 1;
        }
        return collection.GetRange(from, to - from + 1);
    }

    public static Vector2 Scaled(this Vector2 v, Vector2 scale) {
        v.Scale(scale);
        return v;
    }

    public static Vector3 Scaled(this Vector3 v, Vector3 scale) {
        v.Scale(scale);
        return v;
    }

    public static Vector2 Inverse(this Vector2 v) {
        return new Vector2(1 / v.x, 1 / v.y);
    }

    public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max) {
        return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
    }

    public static Vector2 Clamp(this Vector2 v, Vector2 max) {
        return Clamp(v, Vector2.zero, max);
    }

    public static double Rnd(double min, double max) {
        return (double)(UnityEngine.Random.Range((float)min, (float)max));
    }

    public static float GaussianRnd() {
        float sum = 0;
        for (int i = 0; i < 12; i++) {
            sum += UnityEngine.Random.Range(-1f, 1f);
        }
        return sum;
    }

    public static bool Editor(this MonoBehaviour x) {
        return Editor();
    }

    public static bool Editor() {
#if UNITY_EDITOR
        return !EditorApplication.isPlaying;
#else 
        return false;
#endif
    }


    public static void Destroy(this Component c) {
        if (Editor()) {
            UnityEngine.GameObject.DestroyImmediate(c.gameObject);
        } else {
            c.gameObject.SetActive(false);
            UnityEngine.GameObject.Destroy(c.gameObject);
        }
    }

    public static string i(this string s, params object[] args) {
        return string.Format(s, args);
    }

    public static void SetName(this MonoBehaviour mb, string newName) {
        if (mb.name != newName) {
            mb.name = newName;
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(mb, "Auto name change");
#endif
        }
    }

    public static float Part(this UnityEngine.UI.Slider slider) {
        return (slider.value - slider.minValue) / (slider.maxValue - slider.minValue);
    }

    public static string Join<T>(this IEnumerable<T> collection, string separator) {
        return String.Join(separator, collection);
    }

    public static string Join<T>(this IEnumerable<T> collection, char separator) {
        return Join(collection, separator + "");
    }
}