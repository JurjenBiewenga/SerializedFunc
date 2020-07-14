// Simple helper class that allows you to serialize System.Type objects.
// Use it however you like, but crediting or even just contacting the author would be appreciated (Always 
// nice to see people using your stuff!)
//
// Written by Bryan Keiren (http://www.bryankeiren.com)

using System;
using UnityEngine;
using System.Runtime.Serialization;
using UnityEngine.Serialization;

namespace SerializedFuncImpl
{
	[System.Serializable]
	public class SerializableSystemType
	{
		[FormerlySerializedAs("m_Name")]
		[SerializeField]
		private string name;

		public string Name
		{
			get { return name; }
		}

		[FormerlySerializedAs("m_AssemblyQualifiedName")]
		[SerializeField]
		private string assemblyQualifiedName;

		public string AssemblyQualifiedName
		{
			get { return assemblyQualifiedName; }
		}

		[FormerlySerializedAs("m_AssemblyName")]
		[SerializeField]
		private string assemblyName;

		public string AssemblyName
		{
			get { return assemblyName; }
		}

		private System.Type systemType;

		public System.Type SystemType
		{
			get
			{
				if (systemType == null)
				{
					systemType = GetSystemType(assemblyQualifiedName);
				}
				return systemType;
			}
		}

		public static Type GetSystemType(string assemblyQualifiedName)
		{
			return System.Type.GetType(assemblyQualifiedName);
		}

		public SerializableSystemType(System.Type systemType)
		{
			this.systemType = systemType;
			name = systemType.Name;
			assemblyQualifiedName = systemType.AssemblyQualifiedName;
			assemblyName = systemType.Assembly.FullName;
		}

		public override bool Equals(System.Object obj)
		{
			SerializableSystemType temp = obj as SerializableSystemType;
			if ((object)temp == null)
			{
				return false;
			}

			return this.Equals(temp);
		}

		public bool Equals(SerializableSystemType @object)
		{
			return @object.SystemType == SystemType;
		}

		public static bool operator ==(SerializableSystemType a, SerializableSystemType b)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(SerializableSystemType a, SerializableSystemType b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return SystemType != null ? SystemType.GetHashCode() : 0;
		}
	}
}