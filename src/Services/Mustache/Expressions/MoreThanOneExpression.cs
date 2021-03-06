﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Logging;
using Steeltoe.Initializr.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steeltoe.Initializr.Services.Mustache
{
    public class MoreThanOneExpression : IExpression
    {
        private readonly CalculatedParam _param;
        private readonly EvaluationExpression _evaluationExpression;
        private readonly ILogger _logger;

        public MoreThanOneExpression(ILogger logger, CalculatedParam param, MustacheConfigSchema schema)
        {
            _param = param;
            _logger = logger;
            _evaluationExpression = BuildEvaluationExpression(schema);
        }

        public EvaluationExpression BuildEvaluationExpression(MustacheConfigSchema schema)
        {
            // TODO: Add validation
            using (Timing.Over(_logger, "Build MoreThanOne Expression"))
            {
                var keys = _param.Expression.Split(',');
                return dataView =>
                {
                    var moreThanOne = dataView.Count(kvp =>
                                        keys.Contains(kvp.Key)
                                        && bool.TryParse(kvp.Value.ToString(), out var boolValue)
                                        && boolValue) > 1;
                    return moreThanOne.ToString();
                };
            }
        }

        public async Task<string> EvaluateExpressionAsync(Dictionary<string, string> dataView)
        {
            using (Timing.Over(_logger, "Evaluate MoreThanOne Expression"))
            {
                return await Task.Run(() => _evaluationExpression(dataView));
            }
        }
    }
}
