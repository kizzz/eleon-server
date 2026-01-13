using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyRouter.Minimal.HttpApi.Helpers
{
  public class ErrorHandlingHelper
  {
    public static string GenerateErrorHandlingScript()
    {
      return @"
    <style>
      /* Style for the small error button overlay */
      #error-button {
        position: fixed;
        bottom: 30%;
        right: 0px;
        z-index: 1000000;
        background-color: #f44336;
        color: #fff;
        border: none;
        border-radius: 50%;
        width: 50px;
        height: 50px;
        font-size: 16px;
        cursor: pointer;
        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.3);
      }
      #error-button:hover {
        background-color: #d32f2f;
      }

      /* Modal (popup) overlay styling */
      #error-modal {
        display: none; /* Hidden by default */
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 1000001;
        background-color: rgba(0, 0, 0, 0.6);
        overflow-y: auto;
      }
      #error-modal .modal-content {
        background-color: #fff;
        margin: 50px auto;
        padding: 20px;
        border-radius: 5px;
        width: 90%;
        max-width: 600px;
        position: relative;
      }
      #error-modal .close-btn {
        position: absolute;
        top: 10px;
        right: 10px;
        background: none;
        border: none;
        font-size: 18px;
        cursor: pointer;
      }

      /* Styling for error items in the modal */
      .error-item {
        margin-bottom: 15px;
        border-bottom: 1px solid #ccc;
        padding-bottom: 10px;
      }
      .error-text {
        font-weight: bold;
      }
      .error-trace {
        background-color: #f5f5f5;
        padding: 10px;
        margin-top: 5px;
        border-radius: 3px;
        font-family: monospace;
        white-space: pre-wrap;
        display: none;
      }
      .toggle-btn {
        background: none;
        border: none;
        color: #007bff;
        text-decoration: underline;
        cursor: pointer;
        font-size: 12px;
        margin-left: 10px;
      }
    </style>

    <div id=""error-modal"">
      <div class=""modal-content"">
        <button class=""close-btn"">&times;</button>
        <h3>Errors Detected</h3>
        <div id=""error-list""></div>
      </div>
    </div>

    <button id=""error-button"" style=""display:none;"">!</button>

    <script>
      (function () {
        // Ensure window.environment and errors array exist
        window.environment = window.environment || {};
        window.environment.eleoncoreErrors = window.environment.eleoncoreErrors || [];

        // For testing, you can uncomment the following simulated errors:
        /*
        window.environment.eleoncoreErrors.push({
          text: 'Test Error 1: Something went wrong!',
          trace: 'TypeError: undefined is not a function\n    at index.js:42:13'
        });
        window.environment.eleoncoreErrors.push({
          text: 'Test Error 2: Another issue occurred!',
          trace: 'ReferenceError: foo is not defined\n    at index.js:50:5'
        });
        */

        // Renders or updates the error button based on current errors.
        function renderErrorButton() {
          var errors = window.environment.eleoncoreErrors;
          var errorButton = document.getElementById('error-button');
          if (errors && errors.length > 0) {
            errorButton.style.display = 'block';
            // Display the number of errors in the button.
            errorButton.textContent = errors.length;
          } else {
            errorButton.style.display = 'none';
          }
        }

        // Renders the list of errors in the modal popup.
        function renderErrorModal() {
          var errors = window.environment.eleoncoreErrors;
          var errorList = document.getElementById('error-list');
          errorList.innerHTML = '';
          errors.forEach(function (error, index) {
            var errorItem = document.createElement('div');
            errorItem.className = 'error-item';

            var errorText = document.createElement('div');
            errorText.className = 'error-text';
            errorText.textContent = error.text || ('Error ' + (index + 1));
            errorItem.appendChild(errorText);

            // If a trace is available, add a toggle button.
            if (error.trace) {
              var toggleBtn = document.createElement('button');
              toggleBtn.className = 'toggle-btn';
              toggleBtn.textContent = '[Show Trace]';
              errorText.appendChild(toggleBtn);

              var errorTrace = document.createElement('div');
              errorTrace.className = 'error-trace';
              errorTrace.textContent = error.trace;
              errorItem.appendChild(errorTrace);

              toggleBtn.addEventListener('click', function () {
                if (errorTrace.style.display === 'none' || errorTrace.style.display === '') {
                  errorTrace.style.display = 'block';
                  toggleBtn.textContent = '[Hide Trace]';
                } else {
                  errorTrace.style.display = 'none';
                  toggleBtn.textContent = '[Show Trace]';
                }
              });
            }
            errorList.appendChild(errorItem);
          });
        }

        // Show the modal popup.
        function showModal() {
          renderErrorModal();
          document.getElementById('error-modal').style.display = 'block';
        }

        // Hide the modal popup.
        function hideModal() {
          document.getElementById('error-modal').style.display = 'none';
        }

        // Attach click events for the button and modal.
        document.getElementById('error-button').addEventListener('click', showModal);
        document.querySelector('#error-modal .close-btn').addEventListener('click', hideModal);
        // Allow clicking outside the modal content to close it.
        document.getElementById('error-modal').addEventListener('click', function (e) {
          if (e.target === this) {
            hideModal();
          }
        });

        // Initial render of the error button when DOM is ready.
        if (document.readyState === 'loading') {
          document.addEventListener('DOMContentLoaded', renderErrorButton);
        } else {
          renderErrorButton();
        }

        // Expose a helper function to add errors from anywhere in your app.
        window.addEleoncoreError = function (text, trace) {
          window.environment.eleoncoreErrors.push({ text: text, trace: trace });
          renderErrorButton();
        };
      })();
    </script>
            ";
    }
  }
}
