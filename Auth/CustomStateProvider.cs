using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
 
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace EApprove.Auth
{
public class CustomStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    private IConfiguration _configRoot;

    private IEnumerable<Claim>? _claims;
    private string SecretKey;
    public CustomStateProvider(IConfiguration configRoot)
    {
        _configRoot = configRoot;
        SecretKey = _configRoot["AppAuthen:SecretKey"];
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            //var userInfo = await GetCurrentUser();

            if (_claims == null){

                //var userSessionStorageResult = await _sessionStorage.GetAsync<IEnumerable<Claim>>("UserClaims");
                //var _claims = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            }

            if (_claims == null){
               // Console.WriteLine("null");
                return await Task.FromResult(new AuthenticationState(_anonymous));             
            }else{
    
                ClaimsIdentity identity = new ClaimsIdentity(_claims, "AppAuthen","EmployeeId","role");

                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
            }
           // Console.WriteLine("Request GetAuthenticationStateAsync:GetCurrentUser"+userInfo.IsAuthenticated);

        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("GetAuthenticationStateAsync failed:" + ex.ToString());
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public void Logout()
    {
        _claims = null;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
    public async Task<int> Login(string token)
    {
        _claims = DecodeToken(token, SecretKey);
        if (_claims==null) return 0;
        NotifyAuthenticationStateChanged(await Task.FromResult(GetAuthenticationStateAsync()));
        return 1;
    }

    // *** UTIL jwt
    private IEnumerable<Claim>? DecodeToken(string token, string secret)
    {
        if (token == null) 
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
            //var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            //jwtToken.Claims.Append<Claim>( new Claim(ClaimTypes.Name, "Guy"));
            // return user id from JWT token if validation successful
            return jwtToken.Claims;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
}
}
