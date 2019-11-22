﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockify.Net;
using Clockify.Net.Models.Projects;
using Clockify.Net.Models.Tags;
using Clockify.Net.Models.Tasks;
using Clockify.Net.Models.Templates;
using Clockify.Net.Models.Workspaces;
using FluentAssertions;
using NUnit.Framework;

namespace Clockify.Tests {
	public class TemplateTests {
		private readonly ClockifyClient _client;
		private string _workspaceId;

		public TemplateTests() {
			_client = new ClockifyClient();
		}

		[SetUp]
		public async Task Setup() {
			var workspaceResponse =
				await _client.CreateWorkspaceAsync(new WorkspaceRequest { Name = "TemplateWorkspace" });
			workspaceResponse.IsSuccessful.Should().BeTrue();
			_workspaceId = workspaceResponse.Data.Id;
		}

		[TearDown]
		public async Task Cleanup() {
			var workspaceResponse = await _client.DeleteWorkspaceAsync(_workspaceId);
			workspaceResponse.IsSuccessful.Should().BeTrue();
		}

		[Test]
		public async Task FindAllTemplatesOnWorkspaceAsync_ShouldReturnTagsList() {
			var response = await _client.FindAllTemplatesOnWorkspaceAsync(_workspaceId);
			response.IsSuccessful.Should().BeTrue();
		}

		[Test]
		public async Task CreateTemplatesAsync_ShouldCreteTemplateAndReturnTemplateDto() {
			// Create project for test
			var projectRequest = new ProjectRequest { Name = "Template test project", Color = "#FFFFFF" };
			var projectResponse = await _client.CreateProjectAsync(_workspaceId, projectRequest);
			projectResponse.IsSuccessful.Should().BeTrue();
			var projectId = projectResponse.Data.Id;
			// Create task
			var taskRequest = new TaskRequest { Name = "Template create task" };
			var taskResponse = await _client.CreateTaskAsync(_workspaceId, projectId, taskRequest);
			taskResponse.IsSuccessful.Should().BeTrue();
			var taskId = taskResponse.Data.Id;

			var templatePatchRequest = new TemplateRequest() {
				Name = "Test template",
				ProjectsAndTasks = new List<ProjectsTaskTupleRequest>() {
					new ProjectsTaskTupleRequest {
						ProjectId = projectId,
						TaskId = taskId
					}
				}
			};

			var createResult = await _client.CreateTemplatesAsync(_workspaceId, templatePatchRequest);
			createResult.IsSuccessful.Should().BeTrue();
			createResult.Data.Should().NotBeNull();
		}

		[Test]
		public async Task GetTemplatesOnWorkspaceAsync_ShouldReturnTemplateDto() {
			// Create project for test
			var projectRequest = new ProjectRequest { Name = "Template test project", Color = "#FFFFFF" };
			var projectResponse = await _client.CreateProjectAsync(_workspaceId, projectRequest);
			projectResponse.IsSuccessful.Should().BeTrue();
			var projectId = projectResponse.Data.Id;
			// Create task
			var taskRequest = new TaskRequest { Name = "Template create task" };
			var taskResponse = await _client.CreateTaskAsync(_workspaceId, projectId, taskRequest);
			taskResponse.IsSuccessful.Should().BeTrue();
			var taskId = taskResponse.Data.Id;

			var templatePatchRequest = new TemplateRequest() {
				Name = "Test template",
				ProjectsAndTasks = new List<ProjectsTaskTupleRequest>() {
					new ProjectsTaskTupleRequest {
						ProjectId = projectId,
						TaskId = taskId
					}
				}
			};

			var createResult = await _client.CreateTemplatesAsync(_workspaceId, templatePatchRequest);
			createResult.IsSuccessful.Should().BeTrue();
			createResult.Data.Should().NotBeNull();

			var getResponse = await _client.GetTemplatesOnWorkspaceAsync(_workspaceId, createResult.Data.First().Id);
			getResponse.IsSuccessful.Should().BeTrue();
			getResponse.Data.Should().BeEquivalentTo(createResult.Data.First());
		}

		[Test]
		public async Task UpdateTemplateAsync_ShouldReturnTemplateDto() {
			// Create project for test
			var projectRequest = new ProjectRequest { Name = "Template test project", Color = "#FFFFFF" };
			var projectResponse = await _client.CreateProjectAsync(_workspaceId, projectRequest);
			projectResponse.IsSuccessful.Should().BeTrue();
			var projectId = projectResponse.Data.Id;
			// Create task
			var taskRequest = new TaskRequest { Name = "Template create task" };
			var taskResponse = await _client.CreateTaskAsync(_workspaceId, projectId, taskRequest);
			taskResponse.IsSuccessful.Should().BeTrue();
			var taskId = taskResponse.Data.Id;

			var templatePatchRequest = new TemplateRequest() {
				Name = "Test template",
				ProjectsAndTasks = new List<ProjectsTaskTupleRequest>() {
					new ProjectsTaskTupleRequest {
						ProjectId = projectId,
						TaskId = taskId
					}
				}
			};

			var createResult = await _client.CreateTemplatesAsync(_workspaceId, templatePatchRequest);
			createResult.IsSuccessful.Should().BeTrue();
			createResult.Data.Should().NotBeNull();

			var patchRequest = new TemplatePatchRequest { Name = "Updated" };
			var templateDto = createResult.Data.First();
			templateDto.Name = patchRequest.Name;
			var getResponse = await _client.UpdateTemplateAsync(_workspaceId, templateDto.Id, patchRequest);

			getResponse.IsSuccessful.Should().BeTrue();
			getResponse.Data.Should().BeEquivalentTo(templateDto);
		}

		[Test]
		public async Task DeleteTemplateAsync_ShouldReturnTemplateDto() {
			// Create project for test
			var projectRequest = new ProjectRequest { Name = "Template test project", Color = "#FFFFFF" };
			var projectResponse = await _client.CreateProjectAsync(_workspaceId, projectRequest);
			projectResponse.IsSuccessful.Should().BeTrue();
			var projectId = projectResponse.Data.Id;
			// Create task
			var taskRequest = new TaskRequest { Name = "Template create task" };
			var taskResponse = await _client.CreateTaskAsync(_workspaceId, projectId, taskRequest);
			taskResponse.IsSuccessful.Should().BeTrue();
			var taskId = taskResponse.Data.Id;

			var templatePatchRequest = new TemplateRequest() {
				Name = "Test template",
				ProjectsAndTasks = new List<ProjectsTaskTupleRequest>() {
					new ProjectsTaskTupleRequest {
						ProjectId = projectId,
						TaskId = taskId
					}
				}
			};

			var createResult = await _client.CreateTemplatesAsync(_workspaceId, templatePatchRequest);
			createResult.IsSuccessful.Should().BeTrue();
			createResult.Data.Should().NotBeNull();

			var deleteResponse = await _client.DeleteTemplateAsync(_workspaceId, createResult.Data.First().Id);
			deleteResponse.IsSuccessful.Should().BeTrue();
			deleteResponse.Data.Should().BeEquivalentTo(createResult.Data.First());
		}

		[Test]
		public async Task UpdateTemplateAsync_NullTemplatePatchRequest_ShouldThrowArgumentException() {
			Func<Task> create = () => _client.UpdateTemplateAsync(_workspaceId, "", null);
			await create.Should().ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public async Task UpdateTemplateAsync_NullName_ShouldThrowArgumentException() {
			var templatePatchRequest = new TemplatePatchRequest() {
				Name = null,
			};
			Func<Task> create = () => _client.UpdateTemplateAsync(_workspaceId, "", templatePatchRequest);
			await create.Should().ThrowAsync<ArgumentException>()
				.WithMessage($"Argument cannot be null. (Parameter '{nameof(TemplatePatchRequest.Name)}')");
		}

		[Test]
		public async Task CreateTemplatesAsync_NullTemplateRequest_ShouldThrowArgumentException() {
			Func<Task> create = () => _client.CreateTemplatesAsync(_workspaceId, null);
			await create.Should().ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public async Task CreateTemplatesAsync_NullName_ShouldThrowArgumentException() {
			var templatePatchRequest = new TemplateRequest() {
				Name = null,
			};
			Func<Task> create = () => _client.CreateTemplatesAsync(_workspaceId, templatePatchRequest);
			await create.Should().ThrowAsync<ArgumentException>()
				.WithMessage($"Argument cannot be null. (Parameter '{nameof(TemplateRequest.Name)}')");
		}

		[Test]
		public async Task CreateTemplatesAsync_NullProjectsAndTasks_ShouldThrowArgumentException() {
			var templatePatchRequest = new TemplateRequest() {
				Name = "Test name",
				ProjectsAndTasks = null
			};
			Func<Task> create = () => _client.CreateTemplatesAsync(_workspaceId, templatePatchRequest);
			await create.Should().ThrowAsync<ArgumentException>()
				.WithMessage($"Argument cannot be null. (Parameter '{nameof(TemplateRequest.ProjectsAndTasks)}')");
		}

		[Test]
		public async Task CreateTemplatesAsync_NullProjectId_ShouldThrowArgumentException() {
			var templatePatchRequest = new TemplateRequest() {
				Name = "Test name",
				ProjectsAndTasks = new List<ProjectsTaskTupleRequest> {
					new ProjectsTaskTupleRequest {
						ProjectId = null,
						TaskId = "Test"
					}
				}
			};
			Func<Task> create = () => _client.CreateTemplatesAsync(_workspaceId, templatePatchRequest);
			await create.Should().ThrowAsync<ArgumentException>()
				.WithMessage($"Argument cannot be null. (Parameter '{nameof(ProjectsTaskTupleRequest.ProjectId)}')");
		}

		[Test]
		public async Task CreateTemplatesAsync_NullTaskId_ShouldThrowArgumentException() {
			var templatePatchRequest = new TemplateRequest() {
				Name = "Test name",
				ProjectsAndTasks = new List<ProjectsTaskTupleRequest> {
					new ProjectsTaskTupleRequest {
						ProjectId = "Test",
						TaskId = null
					}
				}
			};
			Func<Task> create = () => _client.CreateTemplatesAsync(_workspaceId, templatePatchRequest);
			await create.Should().ThrowAsync<ArgumentException>()
				.WithMessage($"Argument cannot be null. (Parameter '{nameof(ProjectsTaskTupleRequest.TaskId)}')");
		}
	}
}