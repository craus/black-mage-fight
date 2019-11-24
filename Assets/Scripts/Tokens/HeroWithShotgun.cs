using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using RSG;
using System.Collections.Generic;

public class HeroWithShotgun : Hero
{
    public AudioSource shotgunSound;
    public AudioSource explosionSound;

    public GameObject bulletSample;
    public GameObject explosionSample;
    public GameObject lineSample;

	public List<Mark> ignore;

    IPromise PlayBulletFlyAnimation(Cell from, Cell to) {
        Vector3 move = (to.transform.position - from.transform.position);

        var bullet = Instantiate(bulletSample);
        bullet.SetActive(true);
        bullet.transform.position = from.transform.position;
        bullet.transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg);

        var bulletLine = Instantiate(lineSample);
        bulletLine.SetActive(true);
        bulletLine.GetComponentInChildren<LineRenderer>().SetPosition(0, from.transform.position + Vector3.back);
        bulletLine.GetComponentInChildren<LineRenderer>().SetPosition(1, to.transform.position + Vector3.back);
        TimeManager.Wait(0.1f).Then(() => {
            Destroy(bulletLine);
        }).Done();

        return AnimationsManager.Move(bullet.transform, from.transform.position, to.transform.position, 100).Then(() => {
            Destroy(bullet);
        });
    }

    IPromise PlayExplosionAnimation(Cell from) {
        var explosion = Instantiate(explosionSample);
        explosion.SetActive(true);
        explosion.transform.position = from.transform.position;

        return TimeManager.Wait(0.1f).Then(() => {
            Destroy(explosion);
        });
    }

	public bool IsTarget(Figure f) {
		return !f.Marked(ignore);
	}

    public override IPromise<bool> MoveTo(IntVector2 direction) {
		GameManager.instance.BeforeHeroMove();

        var cell = Position;
        var next = cell;
        for (int i = 0; i < 100; i++) {
            next = cell.ToDirection(direction);
			if (next != null && !next.Figures.Any(IsTarget)) {
                cell = next;
            }
        }
        return Promise.Do(() => {
			if (next == null) {
                return Promise.Resolved();
			}
			var target = next.figures[0];
			if (target == null || !target.gameObject.activeSelf) {
				return Promise.Resolved();
			}
			var onHit = target.GetComponentInChildren<OnShotgunHit>();
			if (onHit == null) {
				return Promise.Resolved();
			}
            shotgunSound.Play();
			return PlayBulletFlyAnimation(Position, next).Then(() => {
				onHit.Run();
				return onHit.LongRun();
			});
        }).Then(() => {
            cell = Position.ToDirection(direction);
            if (cell != null) { 
                if (cell.figures.FirstOrDefault() is Crate) {
                    Position.MoveHere(cell.figures.First());
                }      
                var oldPosition = Position;
                cell.MoveHere(this);
                moveSound.Play();
                GameManager.instance.HeroMoved(this, oldPosition, Position, direction);
            }
		}).Return(true);
    }
}
