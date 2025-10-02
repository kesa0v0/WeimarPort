using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 모든 이벤트의 기본이 되는 추상 클래스입니다.
/// 모든 커스텀 이벤트는 이 클래스를 상속받아 정의해야 합니다.
/// 예: public class PlayerActionEvent : GameEvent { ... }
/// </summary>
public abstract class GameEvent { }


/// <summary>
/// 게임 전체에서 이벤트 발행(Publish) 및 구독(Subscribe)을 관리하는 중앙 허브입니다.
/// 정적 클래스로 구현되어 어디서든 EventBus.Publish( ... ) 형태로 쉽게 접근할 수 있습니다.
/// 이 패턴은 각 시스템 간의 직접적인 참조를 없애 결합도를 낮추고 유연한 구조를 만듭니다.
/// </summary>
public static class EventBus
{
    /// <summary>
    /// 이벤트 타입(Type)을 키로, 해당 이벤트에 대한 리스너(Delegate)를 값으로 저장하는 딕셔너리입니다.
    /// Delegate는 여러 메소드(리스너)를 담을 수 있는 컨테이너 역할을 합니다.
    /// </summary>
    private static readonly Dictionary<Type, Delegate> s_events = new Dictionary<Type, Delegate>();

    /// <summary>
    /// 특정 타입의 이벤트가 발생했을 때 호출될 메소드(리스너)를 등록(구독)합니다.
    /// </summary>
    /// <typeparam name="T">구독할 이벤트의 타입. GameEvent를 상속받아야 합니다.</typeparam>
    /// <param name="listener">이벤트가 발생했을 때 실행될 Action<T> 메소드입니다.</param>
    public static void Subscribe<T>(Action<T> listener) where T : GameEvent
    {
        Type eventType = typeof(T);

        // 딕셔너리에 이미 해당 이벤트 타입에 대한 리스너가 등록되어 있는지 확인합니다.
        if (s_events.TryGetValue(eventType, out var existingDelegate))
        {
            // 이미 존재하면, 기존 Delegate에 새로운 리스너를 결합(추가)합니다.
            s_events[eventType] = Delegate.Combine(existingDelegate, listener);
        }
        else
        {
            // 존재하지 않으면, 새로운 키-값 쌍으로 딕셔너리에 추가합니다.
            s_events[eventType] = listener;
        }
    }

    /// <summary>
    /// 등록했던 이벤트 리스너를 제거(구독 취소)합니다.
    /// 주로 오브젝트가 파괴되거나 비활성화될 때(OnDisable, OnDestroy) 호출하여 메모리 누수를 방지합니다.
    /// </summary>
    /// <typeparam name="T">구독 취소할 이벤트의 타입.</typeparam>
    /// <param name="listener">제거할 Action<T> 메소드입니다.</param>
    public static void Unsubscribe<T>(Action<T> listener) where T : GameEvent
    {
        Type eventType = typeof(T);

        // 해당 이벤트 타입이 딕셔너리에 등록되어 있는지 확인합니다.
        if (s_events.TryGetValue(eventType, out var existingDelegate))
        {
            // 기존 Delegate에서 특정 리스너를 제거합니다.
            var resultingDelegate = Delegate.Remove(existingDelegate, listener);

            // 리스너 제거 후 Delegate에 아무것도 남지 않으면 딕셔너리에서 해당 이벤트 키를 삭제합니다.
            if (resultingDelegate == null)
            {
                s_events.Remove(eventType);
            }
            else
            {
                // 리스너가 남아있으면 갱신된 Delegate로 값을 교체합니다.
                s_events[eventType] = resultingDelegate;
            }
        }
    }

    /// <summary>
    /// 특정 이벤트를 시스템 전체에 발행(방송)합니다.
    /// 이 이벤트를 구독하고 있는 모든 리스너들이 즉시 호출됩니다.
    /// </summary>
    /// <typeparam name="T">발행할 이벤트의 타입.</typeparam>
    /// <param name="gameEvent">발행할 이벤트 객체. 이벤트에 필요한 데이터를 담고 있습니다.</param>
    public static void Publish<T>(T gameEvent) where T : GameEvent
    {
        // 발행하려는 이벤트 타입에 대한 리스너가 등록되어 있는지 확인합니다.
        if (s_events.TryGetValue(typeof(T), out var existingDelegate))
        {
            // 리스너가 있다면, 해당 타입의 Action으로 캐스팅하여 Invoke()로 실행합니다.
            // ?. (null-conditional operator)를 사용하여 Delegate가 null이 아닐 때만 호출하도록 합니다.
            (existingDelegate as Action<T>)?.Invoke(gameEvent);
        }
    }
}