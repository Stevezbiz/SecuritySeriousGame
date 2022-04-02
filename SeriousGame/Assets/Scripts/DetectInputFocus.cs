/*
 * Project developed at Politecnico di Torino (2021-2022) by Stefano Gennero
 * in collaboration with prof. Andrea Atzeni and prof. Antonio Lioy.
 * 
 * Copyright 2022 Stefano Gennero
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 *      
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WebGLKeyboard
{
    /// <summary>
    /// Trigger the focus event in input fields
    /// </summary>
    public class DetectInputFocus : MonoBehaviour, IPointerClickHandler, IDeselectHandler
    {
        private KeyboardController controller = null;
        private UnityEngine.UI.InputField nativeInput;
#if USE_TMPRO
        private TMPro.TMP_InputField tmproInput;
#endif

        public void Initialize(KeyboardController _controller)
        {
            controller = _controller;
            nativeInput = gameObject.GetComponent<UnityEngine.UI.InputField>();
#if USE_TMPRO
            tmproInput = gameObject.GetComponent<TMPro.TMP_InputField>();
#endif
        }
        /// <summary>
        /// Calls the controller passing the selected input field to enable the keyboard
        /// </summary>
        /// <param name="_data"></param>
        public void OnPointerClick(PointerEventData _data)
        {
            if (nativeInput != null)
            {
                controller.FocusInput(nativeInput);
            }
#if USE_TMPRO
            if (tmproInput != null)
            {
                controller.FocusInput(tmproInput);
            }
#endif
        }
        /// <summary>
        /// Clears the input action if the player deselected the field ingame
        /// </summary>
        /// <param name="data"></param>
        public void OnDeselect(BaseEventData data)
        {
            controller.ForceClose();
        }
    }
}