#region Header
//
// Test IFC GUID base 64 encoding
//
// Copyright (C) 2009 by Jeremy Tammik, Autodesk, Inc.
// All rights reserved.
//
// The Building Coder, http://thebuildingcoder.typepad.com
//
// The standard base 64 encoding and decoding systems provided 
// by the .NET framework and other short GUID utilities do not
// exactly match the base 64 encoding used by IFC.
//
// This .NET client accesses the original IFC compatible 
// encoding implemented in C and demonstrates round trip
// encoding and decoding.
//
// History:
//
// 2009-02-12 initial implementation
//
#endregion // Header

#region Namespaces
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using ToBase64;
using ShortGuid;
#endregion // Namespaces

namespace IfcGuid
{
  class Program
  {
    [DllImport( "IfcGuid.dll" )]
    static extern void getString64FromGuid( 
      ref Guid guid, 
      StringBuilder s64 );

    [DllImport( "IfcGuid.dll" )]
    static extern bool getGuidFromString64( 
      string s64, 
      ref Guid guid );

    static Guid String64ToGuid( string s64 )
    {
      Guid guid = new Guid();
      //getGuidFromString64( s4, ref guid2 );
      getGuidFromString64( s64.ToString(), ref guid );
      return guid;
    }

    static string GuidToString64( Guid guid )
    {
      StringBuilder s = new StringBuilder( "                        " );
      getString64FromGuid( ref guid, s );
      return s.ToString();
    }

    static void Print( string s ) 
    {
      Console.WriteLine( s );
    }

    static void Main( string[] args )
    {
      Print( "IfcGuid by Jeremy Tammik." );
      if( 0 == args.Length )
      {
        Print( "Do you have a GUID for me?" );
      }
      else
      {
        string a = args[0];
        int n = a.Length;

        if( 45 == n )
        {
          Print( "Revit UniqueId: " + a );

          Guid episodeId = new Guid( a.Substring( 0, 36 ) );

          int elementId = int.Parse( a.Substring( 37 ), 
            NumberStyles.AllowHexSpecifier );

          Print( "     EpisodeId: " + episodeId.ToString() );
          Print( string.Format( "     ElementId: {0} = {1}",
            elementId.ToString(), elementId.ToString( "x8" ) ) );

          int last_32_bits = int.Parse( a.Substring( 28, 8 ), 
            NumberStyles.AllowHexSpecifier );

          int xor = last_32_bits ^ elementId;

          a = a.Substring( 0, 28 ) + xor.ToString( "x8" );

          Print( "          Guid: " + a + "\n" );
        }

        if( 22 == a.Length )
        {
          Guid guid = String64ToGuid( a );
          string s = GuidToString64( guid );
          Print( "Ifc base64 encoding: " + a );
          Print( "    decoded to GUID: " + guid.ToString() );
          Print( "   reencoded to IFC: " + s );
          Debug.Assert( s.Equals( a ), 
            "expected decode + encode to return original guid" );
        }
        else
        {
          Guid guid = new Guid( a );
          string s1 = ToBase64.ToBase64.GetId( guid );
          string s2 = Convert.ToBase64String( guid.ToByteArray() );
          string s3 = new ShortGuid.ShortGuid( guid ).ToString();
          string s4 = GuidToString64( guid );
          Guid guid2 = String64ToGuid( s4 );

          Print( "Original 128-bit GUID: " + guid.ToString() );
          Print( "    ToBase64 encoding: " + s1 );
          Print( " .NET base64 encoding: " + s2 );
          Print( "   ShortGuid encoding: " + s3 );
          Print( " IFC base 64 encoding: " + s4 );
          Print( " decoded back to GUID: " + guid2.ToString() );
          Debug.Assert( guid2.Equals( guid ), 
            "expected encode + decode to return original guid" );
        }
      }
    }
  }
}
