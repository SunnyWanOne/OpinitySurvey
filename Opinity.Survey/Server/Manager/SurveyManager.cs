using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Enums;
using Oqtane.Repository;
using Opinity.Survey.Repository;
using Oqtane.Shared;
using Oqtane.Migrations.Framework;


namespace Opinity.Survey.Manager
{
    public class SurveyManager : MigratableModuleBase, IInstallable, IPortable
    {
        private readonly ISurveyRepository _SurveyRepository;
        private readonly IDBContextDependencies _DBContextDependencies;
        private ISqlRepository _sql;

        public SurveyManager(ISurveyRepository SurveyRepository, IDBContextDependencies DBContextDependencies, ISqlRepository sql)
        {
            _SurveyRepository = SurveyRepository;
            _DBContextDependencies = DBContextDependencies;
            _sql = sql;
        }

        public bool Install(Tenant tenant, string version)
        {
            if (tenant.DBType == Constants.DefaultDBType && version == "2.0.0")
            {
                // prior versions used SQL scripts rather than migrations, so we need to seed the migration history table
                _sql.ExecuteNonQuery(tenant, MigrationUtils.BuildInsertScript("Survey.01.00.00.00"));
                _sql.ExecuteNonQuery(tenant, MigrationUtils.BuildInsertScript("Survey.01.00.02.00"));
            }
            return Migrate(new SurveyContext(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new SurveyContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Module module)
        {
            string content = "";
            List<Models.OqtaneSurvey> Surveys = _SurveyRepository.GetAllSurveysByModule(module.ModuleId).ToList();
            if (Surveys != null)
            {
                content = JsonSerializer.Serialize(Surveys);
            }
            return content;
        }

        public void ImportModule(Module module, string content, string version)
        {
            List<Models.OqtaneSurvey> Surveys = null;
            if (!string.IsNullOrEmpty(content))
            {
                Surveys = JsonSerializer.Deserialize<List<Models.OqtaneSurvey>>(content);
            }
            if (Surveys != null)
            {
                foreach(var Survey in Surveys)
                {
                    _SurveyRepository.CreateSurvey(
                        new Models.OqtaneSurvey
                        {
                            ModuleId = module.ModuleId,
                            SurveyName = Survey.SurveyName,
                            UserId = Survey.UserId
                        });
                }
            }
        }
    }
}
