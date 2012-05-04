using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InsipidusEngine
{
    class Camera2D
    {
        Vector2 _position;
        Vector2 _origPosition;
        Vector2 _targetPosition;
        float _moveSpeed;
        float _rotation;
        float _origRotation;
        float _targetRotation;
        float _zoom;
        float _origZoom;
        float _targetZoom;
        float _zoomRate;
        float _maxZoom;
        float _minZoom;
        float _rotationRate;
        float _transition;
        bool _transitioning;
        const float _transitionSpeed = 0.01f;
        const float _smoothingSpeed = 0.15f;
        Vector2 _size;
        Vector2 _minPosition;
        Vector2 _maxPosition;
        Vector2 _trackingPosition;

        public Camera2D(Vector2 size, Vector2? position, float? moveSpeed, float? rotation,
            float? rotationRate, float? zoom, float? zoomRate, float? minZoom, float? maxZoom,
            Vector2? minPosition, Vector2? maxPosition)
        {
            _position = (position.HasValue) ? position.Value : Vector2.Zero;
            _origPosition = (position.HasValue) ? position.Value : Vector2.Zero;
            _targetPosition = (position.HasValue) ? position.Value : Vector2.Zero;
            _moveSpeed = (moveSpeed.HasValue) ? moveSpeed.Value * 100f : 1;
            _rotation = (rotation.HasValue) ? rotation.Value : 0; 
            _origRotation = (rotation.HasValue) ? rotation.Value : 0;
            _targetRotation = (rotation.HasValue) ? rotation.Value : 0;
            _rotationRate = (rotationRate.HasValue) ? rotationRate.Value : 0.01f;
            _zoom = (zoom.HasValue) ? zoom.Value : 1;
            _origZoom = (zoom.HasValue) ? zoom.Value : 1;
            _targetZoom = (zoom.HasValue) ? zoom.Value : 1;
            _zoomRate = (zoomRate.HasValue) ? zoomRate.Value : 0.01f;
            _minZoom = (minZoom.HasValue) ? minZoom.Value : 0.25f;
            _maxZoom = (maxZoom.HasValue) ? maxZoom.Value : 4f; 
            _size = size;
            _minPosition = (minPosition.HasValue) ? minPosition.Value : Vector2.Zero;
            _maxPosition = (maxPosition.HasValue) ? maxPosition.Value : Vector2.Zero;
        }  
        
        public float ZoomRate
        {
            get { return _zoomRate; }
            set { _zoomRate = value; }
        }
        
        public Vector2 Position
        {
            get { return _position; } 
            set { _position = value; }
        }
        
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }  
        
        public float Zoom 
        {  
            get { return _zoom; }    
            set { _zoom = value; }   
        }     
        
        public float MaxZoom    
        {     
            get { return _maxZoom; }
            set { _maxZoom = value; }   
        }
        
        public float MinZoom     
        {           
            get { return _minZoom; }  
            set { _minZoom = value; }  
        }
        
        public float RotationRate
        {    
            get { return _rotationRate; }
            set { _rotationRate = value; }
        }
        
        public Vector2 Size
        {
            get { return _size; }   
            set { _size = value; }  
        }
        
        public Vector2 CurSize
        {
            get { return Vector2.Multiply(_size, 1 / _zoom); }
        }
        
        public Matrix CameraMatrix
        {
            get
            {
                return Matrix.Identity * Matrix.CreateTranslation(new Vector3(-_position, 0)) *
                Matrix.CreateScale(_zoom) * Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateTranslation(new Vector3(_size / 2, 0));
            }
        }
        
        public Vector2 MinPosition
        {       
            get { return _minPosition; }   
            set { _minPosition = value; }   
        }   
        
        public Vector2 MaxPosition     
        {        
            get { return _maxPosition; }  
            set { _maxPosition = value; } 
        }    
        
        public Vector2 TrackingPosition    
        {        
            get { return _trackingPosition; }
            set { _trackingPosition = value; }   
        }
        
        public void Update(InputHelper input)  
        {     
            if (!_transitioning)  
            {  
                if (_trackingPosition == null)   
                {      
                    if (_minPosition.X != _maxPosition.X || _minPosition.Y != _maxPosition.Y) 
                    {
                        _targetPosition =
                            Vector2.Clamp(_position + new Vector2((input.RightStick.X * _moveSpeed) * _zoom,
                                -(input.RightStick.Y * _moveSpeed) * _zoom), _minPosition, _maxPosition);
                    }
                    else 
                    {
                        _targetPosition += new Vector2((input.RightStick.X * _moveSpeed) * _zoom,
                            -(input.RightStick.Y * _moveSpeed) * _zoom); 
                    }
                }    
                else        
                {        
                    if (_minPosition.X != _maxPosition.X || _minPosition.Y != _maxPosition.Y) 
                    {
                        _targetPosition = Vector2.Clamp(_trackingPosition, _minPosition, _maxPosition);
                    }
                    else
                    {
                        _targetPosition = _trackingPosition;
                    }
                }        
                if (input.IsCurPress(Buttons.DPadUp))
                {
                    _targetZoom = Math.Min(_maxZoom, _zoom + _zoomRate); 
                }
                if (input.IsCurPress(Buttons.DPadDown))
                {
                    _targetZoom = Math.Max(_minZoom, _zoom - _zoomRate);
                }
                
                //uncomment this to enable rotation
                /*
                 * if (input.IsCurPress(Buttons.DPadLeft))
                 * {
                 * _targetRotation = (_rotation + _rotationRate) % (float)(Math.PI * 2);
                 * }
                 * if (input.IsCurPress(Buttons.DPadRight))
                 * {
                 * _targetRotation = (_rotation - _rotationRate) % (float)(Math.PI * 2);
                 * */

                if (input.IsCurPress(Buttons.RightStick))
                {
                    _transitioning = true;
                    _targetPosition = _origPosition;
                    _targetRotation = _origRotation;
                    _targetZoom = _origZoom;
                    _trackingPosition = Vector2.Zero;
                }
            }
            else if (_transition < 1)
            {
                _transition += _transitionSpeed;
            }
            if (_transition >= 1f ||
                (_position == _origPosition && _rotation == _origRotation && _zoom == _origZoom))
            {
                _transition = 0;
                _transitioning = false;
            }

            _position = Vector2.SmoothStep(_position, _targetPosition, _smoothingSpeed);
            _rotation = MathHelper.SmoothStep(_rotation, _targetRotation, _smoothingSpeed);
            _zoom = MathHelper.SmoothStep(_zoom, _targetZoom, _smoothingSpeed);
        }
    }
}