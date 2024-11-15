using System.Text.Json.Serialization;

public class DoctorInfoDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("dados")]
    public List<DoctorDataDto> Dados { get; set; }
}

public class DoctorDataDto
{
    [JsonPropertyName("COUNT")]
    public string COUNT { get; set; }

    [JsonPropertyName("SG_UF")]
    public string SG_UF { get; set; }

    [JsonPropertyName("NU_CRM")]
    public string NU_CRM { get; set; }

    [JsonPropertyName("NU_CRM_NATURAL")]
    public string NU_CRM_NATURAL { get; set; }

    [JsonPropertyName("NM_MEDICO")]
    public string NM_MEDICO { get; set; }

    [JsonPropertyName("COD_SITUACAO")]
    public string COD_SITUACAO { get; set; }

    [JsonPropertyName("NM_SOCIAL")]
    public string NM_SOCIAL { get; set; }

    [JsonPropertyName("DT_INSCRICAO")]
    public string DT_INSCRICAO { get; set; }

    [JsonPropertyName("IN_TIPO_INSCRICAO")]
    public string IN_TIPO_INSCRICAO { get; set; }

    [JsonPropertyName("TIPO_INSCRICAO")]
    public string TIPO_INSCRICAO { get; set; }

    [JsonPropertyName("SITUACAO")]
    public string SITUACAO { get; set; }

    [JsonPropertyName("ESPECIALIDADE")]
    public string ESPECIALIDADE { get; set; }

    [JsonPropertyName("PRIM_INSCRICAO_UF")]
    public string PRIM_INSCRICAO_UF { get; set; }

    [JsonPropertyName("PERIODO_I")]
    public string PERIODO_I { get; set; }

    [JsonPropertyName("PERIODO_F")]
    public string PERIODO_F { get; set; }

    [JsonPropertyName("OBS_INTERDICAO")]
    public string OBS_INTERDICAO { get; set; }

    [JsonPropertyName("RNUM")]
    public string RNUM { get; set; }

    [JsonPropertyName("SECURITYHASH")]
    public string SECURITYHASH { get; set; }
}
