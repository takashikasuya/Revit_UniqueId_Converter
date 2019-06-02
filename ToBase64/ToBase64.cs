using System;
using System.Collections.Generic;
using System.Text;

namespace ToBase64
{
  public class ToBase64
  {
    public static string GetId( Guid guid )
    {
      return ToBase64String( guid.ToByteArray() );
    }

    public static string GetId()
    {
      return ToBase64String( Guid.NewGuid().ToByteArray() );
    }

    public static readonly char[] base64Chars = new char[]
    { '0','1','2','3','4','5','6','7','8','9'
    , 'A','B','C','D','E','F','G','H','I','J'
    , 'K','L','M','N','O','P','Q','R','S','T'
    , 'U','V','W','X','Y','Z','a','b','c','d'
    , 'e','f','g','h','i','j','k','l','m','n'
    , 'o','p','q','r','s','t','u','v','w','x'
    , 'y','z','_','$' };

    public static string ToBase64String( byte[] value )
    {
      int numBlocks;
      int padBytes;

      if( ( value.Length % 3 ) == 0 )
      {
        numBlocks = value.Length / 3;
        padBytes = 0;
      }
      else
      {
        numBlocks = 1 + ( value.Length / 3 );
        padBytes = 3 - ( value.Length % 3 );
      }
      if( padBytes < 0 || padBytes > 3 )
        throw new ApplicationException( "Fatal logic error in padding code" );

      byte[] newValue = new byte[numBlocks * 3];
      for( int i = 0; i < value.Length; ++i )
        newValue[i] = value[i];

      byte[] resultBytes = new byte[numBlocks * 4];

      for( int i = 0; i < numBlocks; i++ )
      {
        resultBytes[i * 4 + 0] =
             ( byte ) ( ( newValue[i * 3 + 0] & 0xFC ) >> 2 );
        resultBytes[i * 4 + 1] =
             ( byte ) ( ( newValue[i * 3 + 0] & 0x03 ) << 4 |
             ( newValue[i * 3 + 1] & 0xF0 ) >> 4 );
        resultBytes[i * 4 + 2] =
              ( byte ) ( ( newValue[i * 3 + 1] & 0x0F ) << 2 |
              ( newValue[i * 3 + 2] & 0xC0 ) >> 6 );
        resultBytes[i * 4 + 3] =
              ( byte ) ( ( newValue[i * 3 + 2] & 0x3F ) );
      }

      char[] resultChars = new char[numBlocks * 4];

      for( int i = 0; i < numBlocks * 4; ++i )
        resultChars[i] = base64Chars[resultBytes[i]];

      string s = new string( resultChars );
      return s.Substring( 0, 22 );
    }
  }
}
